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

        private Action<BeatEvent> beatEventCallback;
        private Action<ScoreBall> spawnScoreBallEvent;
        private ScoreBall tempScoreBall;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitEventRegisterMock();
            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();

            InitGameSettingMock();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            scoreBallHandlerPresenter = Substitute.For<IScoreBallHandlerPresenter>();
            Container.Bind<IScoreBallHandlerPresenter>().FromInstance(scoreBallHandlerPresenter).AsSingle();

            eventInvoker = Substitute.For<IEventInvoker>();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

            viewManager = Substitute.For<IViewManager>();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();

            appProcessor = Substitute.For<IAppProcessor>();
            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();

            Container.Bind<ScoreBallHandler>().AsSingle();
            scoreBallHandler = Container.Resolve<ScoreBallHandler>();

            InitSpawnScoreBallEventMock();
            scoreBallHandler.OnSpawnScoreBall += spawnScoreBallEvent;
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

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenStartCountDownValueSetting(20);
        }

        private void InitSpawnScoreBallEventMock()
        {
            spawnScoreBallEvent = Substitute.For<Action<ScoreBall>>();
            tempScoreBall = null;

            spawnScoreBallEvent.When(x => x.Invoke(Arg.Any<ScoreBall>())).Do(callInfo =>
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
                spawnScoreBallEvent.DidNotReceive().Invoke(Arg.Any<ScoreBall>());
            else
                spawnScoreBallEvent.Received(expectedCallTimes).Invoke(Arg.Any<ScoreBall>());
        }

        //Beat時, 若沒有進行到需要生成分數球的節拍, 則不做事
        //若生成分數球節拍設定為空, 則報錯
    }
}