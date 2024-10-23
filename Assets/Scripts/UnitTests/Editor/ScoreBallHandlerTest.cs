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
        private IBeaterModel beaterModel;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private IGameSetting gameSetting;
        private IViewManager viewManager;

        private Action<BeatEvent> beatEventCallback;
        private Action spawnScoreBallEvent;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitEventRegisterMock();
            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();

            scoreBallHandlerPresenter = Substitute.For<IScoreBallHandlerPresenter>();
            Container.Bind<IScoreBallHandlerPresenter>().FromInstance(scoreBallHandlerPresenter).AsSingle();

            eventInvoker = Substitute.For<IEventInvoker>();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

            gameSetting = Substitute.For<IGameSetting>();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            viewManager = Substitute.For<IViewManager>();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();

            beaterModel = Substitute.For<IBeaterModel>();
            Container.Bind<IBeaterModel>().FromInstance(beaterModel).AsSingle();

            Container.Bind<ScoreBallHandler>().AsSingle();
            scoreBallHandler = Container.Resolve<ScoreBallHandler>();

            spawnScoreBallEvent = Substitute.For<Action>();
            scoreBallHandler.OnSpawnScoreBall += spawnScoreBallEvent;
        }

        [Test]
        //Beat時, 若進行到需要生成分數球的節拍, 則生成一次分數球
        public void spawn_score_ball_when_beat_and_reach_freq()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 3, 7 });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback(); //1
            CallBeatEventCallback(); //2
            
            ShouldSpawnScoreBall(0);
            
            CallBeatEventCallback(); //3*
            
            ShouldSpawnScoreBall(1);
            
            CallBeatEventCallback(); //4
            CallBeatEventCallback(); //5
            CallBeatEventCallback(); //6
            
            ShouldSpawnScoreBall(1);
            
            CallBeatEventCallback(); //7*
            
            ShouldSpawnScoreBall(2);
        }

        private void InitEventRegisterMock()
        {
            beatEventCallback = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback = callback;
            });
        }

        private void GivenSpawnScoreBallBeatSetting(List<int> spawnBeatIndexList)
        {
            StageSettingContent settingContent = new StageSettingContent();
            settingContent.SetSpawnBeatIndexList(spawnBeatIndexList);

            beaterModel.CurrentStageSettingContent.Returns(settingContent);
        }

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent(false));
        }

        private void ShouldSpawnScoreBall(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                spawnScoreBallEvent.DidNotReceive().Invoke();
            else
                spawnScoreBallEvent.Received(expectedCallTimes).Invoke();
        }

        //Beat時, 若沒有進行到需要生成分數球的節拍, 則不做事
        //若生成分數球節拍設定為空, 則報錯
    }
}