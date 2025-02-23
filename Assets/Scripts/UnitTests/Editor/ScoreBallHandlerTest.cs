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
        private IFeverEnergyBarModel feverEnergyBarModel;
        private IStageSettingContent stageSettingContent;

        private Action<BeatEvent> beatEventCallback;
        private Action<ScoreBall> spawnScoreBallEventCallback;
        private Action initEventCallback;
        private Action releaseEventCallback;
        private ScoreBall tempScoreBall;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitAppProcessorMock();
            InitEventRegisterMock();
            InitGameSettingMock();
            InitScoreBallHandlerPresenterMock();
            InitEventInvokerMock();
            InitViewManagerMock();
            InitBeaterModelMock();
            InitFeverEnergyBarModelMock();

            Container.Bind<ScoreBallHandler>().AsSingle();
            scoreBallHandler = Container.Resolve<ScoreBallHandler>();

            InitSpawnScoreBallEventMock();
            scoreBallHandler.OnSpawnScoreBall += spawnScoreBallEventCallback;

            initEventCallback = Substitute.For<Action>();
            scoreBallHandler.OnInit += initEventCallback;

            releaseEventCallback = Substitute.For<Action>();
            scoreBallHandler.OnRelease += releaseEventCallback;
        }

        private void InitFeverEnergyBarModelMock()
        {
            feverEnergyBarModel = Substitute.For<IFeverEnergyBarModel>();

            GivenCurrentFeverStage(0);

            Container.Bind<IFeverEnergyBarModel>().FromInstance(feverEnergyBarModel).AsSingle();
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
            stageSettingContent = Substitute.For<IStageSettingContent>();

            appProcessor.CurrentStageSettingContent.Returns(stageSettingContent);

            GivenSpawnScoreBallBeatSetting(new List<int> { 0 });

            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenStartCountDownValueSetting(20);
            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 1, 1 }
            });

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

        private void GivenCurrentFeverStage(int feverStage)
        {
            feverEnergyBarModel.CurrentFeverStage.Returns(feverStage);
        }

        private void GivenStartCountDownValueSetting(int startCountDownValue)
        {
            stageSettingContent.ScoreBallStartCountDownValue.Returns(startCountDownValue);
        }

        private void GivenSpawnScoreBallBeatSetting(List<int> spawnBeatIndexList)
        {
            stageSettingContent.SpawnBeatIndexList.Returns(spawnBeatIndexList);
        }

        private void GivenScoreBallFlagWeightSettingIsEmpty()
        {
            stageSettingContent.GetScoreBallFlagWeightSetting(Arg.Any<int>()).Returns(new List<ScoreBallFlagWeightDefine>());
        }

        private void GivenScoreBallFlagWeightSetting(int currentFeverStage, Dictionary<int, int> flagWeightSetting)
        {
            List<ScoreBallFlagWeightDefine> settings = new List<ScoreBallFlagWeightDefine>();
            if (flagWeightSetting != null)
            {
                foreach ((int flagNumber, int weight) in flagWeightSetting)
                {
                    settings.Add(new ScoreBallFlagWeightDefine(flagNumber, weight));
                }
            }

            stageSettingContent.GetScoreBallFlagWeightSetting(currentFeverStage).Returns(settings);
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

        private void SpawnedScoreBallFlagNumberShouldBe(int expectedFlagNumber)
        {
            Assert.AreEqual(expectedFlagNumber, tempScoreBall.CurrentFlagNumber);
        }

        private List<int> CreateList(int length)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }

            return list;
        }

        #region 生成分數球

        [Test]
        //若生成分數球節拍設定為空, 則報錯
        public void throw_exception_when_spawn_beat_setting_is_empty()
        {
            GivenSpawnScoreBallBeatSetting(new List<int>());

            Assert.Throws<NullReferenceException>(scoreBallHandler.ExecuteModelInit);
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
        //Beat時, 若沒有進行到需要生成分數球的節拍, 則不做事
        public void do_nothing_when_beat_and_not_reach_freq()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 3 });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback(); //0
            CallBeatEventCallback(); //1
            CallBeatEventCallback(); //2

            ShouldSpawnScoreBall(0);
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

        #endregion

        #region 發送事件

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
        //生成分數球時, 發送生成分數球事件
        public void send_spawn_score_ball_event_when_spawn_score_ball()
        {
            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback();

            ShouldSpawnScoreBall(1);
        }

        #endregion

        #region 捕獲旗標

        [Test]
        //生成分數球, 依據當前Fever階段對應的旗標編號權重設定, 設定當前旗標編號
        public void spawn_score_ball_and_set_flag_number_by_weight_setting()
        {
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 }
            });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback();

            SpawnedScoreBallFlagNumberShouldBe(4);
        }

        [Test]
        //生成分數球, 依據當前Fever階段對應的旗標編號權重設定, 若有多個權重設定則按照權重隨機抽取旗標編號
        public void spawn_score_ball_and_set_random_flag_number_by_multiple_weight_setting()
        {
            GivenSpawnScoreBallBeatSetting(CreateList(300));
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 },
                { 5, 3 },
                { 6, 6 }
            });

            scoreBallHandler.ExecuteModelInit();

            Dictionary<int, int> flagNumberCounterDict = new Dictionary<int, int>();
            for (int i = 0; i < 300; i++)
            {
                CallBeatEventCallback();

                if (flagNumberCounterDict.ContainsKey(tempScoreBall.CurrentFlagNumber))
                    flagNumberCounterDict[tempScoreBall.CurrentFlagNumber]++;
                else
                    flagNumberCounterDict[tempScoreBall.CurrentFlagNumber] = 1;
            }

            Assert.IsTrue(flagNumberCounterDict[4] > 0);
            Assert.IsTrue(flagNumberCounterDict[5] > 0);
            Assert.IsTrue(flagNumberCounterDict[6] > 0);
            Assert.IsTrue(flagNumberCounterDict[6] > flagNumberCounterDict[5]);
            Assert.IsTrue(flagNumberCounterDict[5] > flagNumberCounterDict[4]);
        }

        [Test]
        //生成分數球, 若取不到旗標編號權重設定則回應錯誤
        public void throw_exception_when_spawn_score_ball_and_no_have_weight_setting()
        {
            GivenScoreBallFlagWeightSettingIsEmpty();

            scoreBallHandler.ExecuteModelInit();

            Assert.Throws<NullReferenceException>(CallBeatEventCallback);
        }

        [Test]
        //結算或倒數完畢後再度激活, 重新抽取旗標編號
        public void reactivate_then_reselect_flag_number()
        {
            GivenSpawnScoreBallBeatSetting(new List<int> { 0, 1 });
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 },
            });

            scoreBallHandler.ExecuteModelInit();

            CallBeatEventCallback();

            int originalScoreBallHashCode = tempScoreBall.GetHashCode();
            SpawnedScoreBallFlagNumberShouldBe(4);
            InFieldScoreBallAmountShouldBe(1);

            tempScoreBall.SuccessSettle();

            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 3, 1 },
            });
            CallBeatEventCallback();

            int newScoreBallHashCode = tempScoreBall.GetHashCode();
            SpawnedScoreBallFlagNumberShouldBe(3);
            InFieldScoreBallAmountShouldBe(1);
            Assert.IsTrue(originalScoreBallHashCode == newScoreBallHashCode);
        }

        #endregion
    }
}