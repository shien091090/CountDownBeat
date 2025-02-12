using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    public class ScoreBallHandlerTest : ZenjectUnitTestFixture
    {
        private ScoreBallHandler scoreBallHandler;
        private IScoreBallHandlerPresenter scoreBallHandlerPresenter;
        private IAppProcessor appProcessor;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private IGameSetting gameSetting;
        private IViewManager viewManager;
        private IBeaterModel beaterModel;

        private Action<BeatEvent> beatEventCallback;
        private Action<ScoreBall> spawnScoreBallEventCallback;
        private Action initEventCallback;
        private Action releaseEventCallback;
        private ScoreBall tempScoreBall;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitEventRegisterMock();
            InitGameSettingMock();
            InitScoreBallHandlerPresenterMock();
            InitEventInvokerMock();
            InitViewManagerMock();
            InitAppProcessorMock();
            InitBeaterModelMock();

            Container.Bind<ScoreBallHandler>().AsSingle();
            scoreBallHandler = Container.Resolve<ScoreBallHandler>();

            InitSpawnScoreBallEventMock();
            scoreBallHandler.OnSpawnScoreBall += spawnScoreBallEventCallback;

            initEventCallback = Substitute.For<Action>();
            scoreBallHandler.OnInit += initEventCallback;

            releaseEventCallback = Substitute.For<Action>();
            scoreBallHandler.OnRelease += releaseEventCallback;
        }

        [Test]
        //初始化時, 發送初始化事件
        public void send_init_event_when_execute_model_init()
        {
            scoreBallHandler.ExecuteModelInit();

            ShouldSendInitEvent(1);
        }

        [Test]
        //釋放時, 發送釋放事件
        public void send_release_event_when_execute_model_release()
        {
            scoreBallHandler.ExecuteModelInit();
            scoreBallHandler.Release();

            ShouldSendReleaseEvent(1);
        }

        [Test]
        //Beat時, 若進行到需要生成分數球的節拍, 則生成一次分數球
        public void spawn_score_ball_when_beat_and_reach_freq()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 0, 3, 7 });

            scoreBallHandler.ExecuteModelInit();

            ShouldSpawnScoreBall(0);

            CallBeatEventCallback(); //0*

            ShouldSpawnScoreBall(1);

            CallBeatEventCallback(); //1
            CallBeatEventCallback(); //2

            ShouldSpawnScoreBall(1);

            CallBeatEventCallback(); //3*

            ShouldSpawnScoreBall(2);

            CallBeatEventCallback(); //4
            CallBeatEventCallback(); //5
            CallBeatEventCallback(); //6

            ShouldSpawnScoreBall(2);

            CallBeatEventCallback(); //7*

            ShouldSpawnScoreBall(3);
        }

        [Test]
        //生成分數球時, 若有隱藏的分數球, 則重新顯示該分數球
        public void reactivate_hidden_score_ball_when_spawn_score_ball()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 0, 1 });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback();

            InFieldScoreBallAmountShouldBe(1);

            tempScoreBall.SuccessSettle();
            SpawnedScoreBallStateShouldBe(ScoreBallState.Hide);

            CallBeatEventCallback();

            InFieldScoreBallAmountShouldBe(1);
            SpawnedScoreBallStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //生成分數球時, 若沒有隱藏的分數球, 則生成新的分數球
        public void spawn_new_score_ball_when_no_hidden_score_ball()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 0, 1 });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback();

            InFieldScoreBallAmountShouldBe(1);
            SpawnedScoreBallStateShouldBe(ScoreBallState.InCountDown);

            CallBeatEventCallback();

            InFieldScoreBallAmountShouldBe(2);
            SpawnedScoreBallStateShouldBe(ScoreBallState.InCountDown);
        }

        private void InitBeaterModelMock()
        {
            beaterModel = Substitute.For<IBeaterModel>();
            Container.Bind<IBeaterModel>().FromInstance(beaterModel).AsSingle();
        }

        private void InitViewManagerMock()
        {
            viewManager = Substitute.For<IViewManager>();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();
        }

        private void InitEventInvokerMock()
        {
            eventInvoker = Substitute.For<IEventInvoker>();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();
        }

        private void InitScoreBallHandlerPresenterMock()
        {
            scoreBallHandlerPresenter = Substitute.For<IScoreBallHandlerPresenter>();
            Container.Bind<IScoreBallHandlerPresenter>().FromInstance(scoreBallHandlerPresenter).AsSingle();
        }

        private void InitAppProcessorMock()
        {
            appProcessor = Substitute.For<IAppProcessor>();

            GivenSpawnScoreBallBeatSetting(new List<int>());

            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenStartCountDownValueSetting(20);

            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();
        }

        private void InitSpawnScoreBallEventMock()
        {
            spawnScoreBallEventCallback = Substitute.For<Action<ScoreBall>>();
            tempScoreBall = null;

            spawnScoreBallEventCallback.When(x => x.Invoke(Arg.Any<ScoreBall>())).Do(callInfo =>
            {
                tempScoreBall = (ScoreBall)callInfo.Args()[0];
            });
        }

        private void InitEventRegisterMock()
        {
            beatEventCallback = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback += callback;
            });

            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();
        }

        private void GivenStartCountDownValueSetting(int startCountDownValue)
        {
            gameSetting.ScoreBallStartCountDownValue.Returns(startCountDownValue);
        }

        private void GivenSpawnScoreBallBeatSetting(List<int> spawnBeatIndexList)
        {
            StageSettingContent settingContent = new StageSettingContent();
            settingContent.SetSpawnBeatIndexList(spawnBeatIndexList);

            appProcessor.CurrentStageSettingContent.Returns(settingContent);
        }

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent(false));
        }

        private void ShouldSendInitEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                initEventCallback.DidNotReceive().Invoke();
            else
                initEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendReleaseEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                releaseEventCallback.DidNotReceive().Invoke();
            else
                releaseEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void SpawnedScoreBallStateShouldBe(ScoreBallState expectedState)
        {
            Assert.AreEqual(expectedState, tempScoreBall.CurrentState);
        }

        private void InFieldScoreBallAmountShouldBe(int expectedAmount)
        {
            Assert.AreEqual(expectedAmount, scoreBallHandler.CurrentInFieldScoreBallAmount);
        }

        private void ShouldSpawnScoreBall(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                spawnScoreBallEventCallback.DidNotReceive().Invoke(Arg.Any<ScoreBall>());
            else
                spawnScoreBallEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<ScoreBall>());
        }

        //Beat時, 若沒有進行到需要生成分數球的節拍, 則不做事
        //若生成分數球節拍設定為空, 則報錯
    }
}