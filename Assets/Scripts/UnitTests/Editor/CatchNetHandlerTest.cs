using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore.UnitTests
{
    public class CatchNetHandlerTest : ZenjectUnitTestFixture
    {
        private CatchNetHandler catchNetHandler;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private IGameSetting gameSetting;
        private ICatchNetHandlerPresenter presenter;
        private ICatchNetView catchNetView;
        private IFeverEnergyBarModel feverEnergyBarModel;
        private IScoreBallHandler scoreBallHandler;

        private Action<BeatEvent> beatEventCallback;
        private Action<ICatchNet> spawnCatchNetEventCallback;
        private Action initEventCallback;
        private Action releaseEventCallback;
        private Action<ICatchNetPresenter> settleCatchNetEventCallback;
        private GetScoreEvent getScoreEvent;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitGameSettingMock();
            InitCatchNetPresenterMock();
            InitEventHandlerMock();
            InitFeverEnergyBarModelMock();
            InitScoreBallHandlerMock();

            Container.Bind<CatchNetHandler>().AsSingle();
            catchNetHandler = Container.Resolve<CatchNetHandler>();

            spawnCatchNetEventCallback = Substitute.For<Action<ICatchNet>>();
            catchNetHandler.OnSpawnCatchNet += spawnCatchNetEventCallback;

            initEventCallback = Substitute.For<Action>();
            catchNetHandler.OnInit += initEventCallback;

            releaseEventCallback = Substitute.For<Action>();
            catchNetHandler.OnRelease += releaseEventCallback;

            settleCatchNetEventCallback = Substitute.For<Action<ICatchNetPresenter>>();
            catchNetHandler.OnSettleCatchNet += settleCatchNetEventCallback;
        }

        private void InitScoreBallHandlerMock()
        {
            scoreBallHandler = Substitute.For<IScoreBallHandler>();

            Container.Bind<IScoreBallHandler>().FromInstance(scoreBallHandler).AsSingle();
        }

        private void InitFeverEnergyBarModelMock()
        {
            feverEnergyBarModel = Substitute.For<IFeverEnergyBarModel>();

            GivenCurrentFeverStage(0);

            Container.Bind<IFeverEnergyBarModel>().FromInstance(feverEnergyBarModel).AsSingle();
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenCatchNetLimitByFeverStageSetting(null);
            GivenScoreBallFlagWeightSetting(Arg.Any<int>(), new Dictionary<int, int>
            {
                { 1, 1 }
            });

            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();
        }

        private void InitCatchNetPresenterMock()
        {
            presenter = Substitute.For<ICatchNetHandlerPresenter>();

            catchNetView = Substitute.For<ICatchNetView>();
            presenter.Spawn(Arg.Any<int>()).Returns(catchNetView);

            GivenTryOccupyPosSuccess(0, CatchNetSpawnFadeInMode.FromBottom);

            Container.Bind<ICatchNetHandlerPresenter>().FromInstance(presenter).AsSingle();
        }

        private void InitEventHandlerMock()
        {
            beatEventCallback = null;
            getScoreEvent = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback += callback;
            });

            eventRegister.When(x => x.Unregister(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback -= callback;
            });

            eventInvoker = Substitute.For<IEventInvoker>();

            eventInvoker.When(x => x.SendEvent(Arg.Any<GetScoreEvent>())).Do(x =>
            {
                getScoreEvent = (GetScoreEvent)x.Args()[0];
            });

            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();
        }

        private void GivenCatchNetLimitByFeverStageSetting(Dictionary<int, int> limitByFeverStageSetting)
        {
            gameSetting.CatchNetLimitByFeverStageSetting.Returns(limitByFeverStageSetting);
        }

        private void GivenScoreBallFlagWeightSetting(int currentFeverStage, Dictionary<int, int> flagWeightSetting)
        {
            gameSetting.GetScoreBallFlagWeightSetting(currentFeverStage).Returns(flagWeightSetting);
        }

        private void GivenTryOccupyPosSuccess(int spawnPosIndex, CatchNetSpawnFadeInMode fadeInMode)
        {
            presenter.TryOccupyPos(out int _, out CatchNetSpawnFadeInMode _).ReturnsForAnyArgs(x =>
            {
                x[0] = spawnPosIndex;
                x[1] = fadeInMode;
                return true;
            });
        }

        private void GivenScoreWhenSuccessSettle(int score)
        {
            gameSetting.SuccessSettleScore.Returns(score);
        }

        private void GivenCatchNetNumberRange(int min, int max)
        {
            gameSetting.CatchNetNumberRange.Returns(new Vector2Int(min, max));
        }

        private void GivenCurrentFeverStage(int feverStage)
        {
            feverEnergyBarModel.CurrentFeverStage.Returns(feverStage);
        }

        private void GivenCurrentInFieldScoreBallAmount(int amount)
        {
            scoreBallHandler.CurrentInFieldScoreBallAmount.Returns(amount);
        }

        private void GivenInFieldScoreBallContainsFlagNumber(int flagNumber, bool isContains)
        {
            scoreBallHandler.IsInFieldScoreBallContainsFlagNumber(flagNumber).Returns(isContains);
        }

        private void CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet)
        {
            CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
            arg.TryTriggerCatch(arg.TargetFlagNumber);

            lastCatchNet = arg;
        }

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent(false));
        }

        private void ShouldSendSettleCatchNetEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                settleCatchNetEventCallback.DidNotReceive().Invoke(Arg.Any<ICatchNetPresenter>());
            else
                settleCatchNetEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<ICatchNetPresenter>());
        }

        private void ShouldSendReleaseEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                releaseEventCallback.DidNotReceive().Invoke();
            else
                releaseEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendInitEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                initEventCallback.DidNotReceive().Invoke();
            else
                initEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void LastGetScoreEventShouldBe(int expectedScore)
        {
            Assert.AreEqual(expectedScore, getScoreEvent.Score);
        }

        private void CatchNetStateShouldBe(CatchNet catchNet, CatchNetState expectedState)
        {
            Assert.AreEqual(expectedState, catchNet.CurrentState);
        }

        private void LastSpawnCatchNetStateShouldBe(CatchNetState expectedState)
        {
            CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
            Assert.AreEqual(expectedState, arg.CurrentState);
        }

        private void LastSpawnCatchNetTargetFlagShouldBe(int expectedFlagNum)
        {
            CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
            Assert.AreEqual(expectedFlagNum, arg.TargetFlagNumber);
        }

        private void CurrentInFieldCatchNetAmountShouldBe(int expectedAmount)
        {
            Assert.AreEqual(expectedAmount, catchNetHandler.CurrentInFieldCatchNetAmount);
        }

        private void ShouldSpawnCatchNet(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                spawnCatchNetEventCallback.DidNotReceive().Invoke(Arg.Any<ICatchNet>());
            else
                spawnCatchNetEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<ICatchNet>());
        }

        private void CurrentCatchNetLimitShouldBe(int expectedLimit)
        {
            Assert.AreEqual(expectedLimit, catchNetHandler.CurrentCatchNetLimit);
        }

        #region 發送事件

        [Test]
        //初始化時, 發送初始化事件
        public void send_init_event_when_init()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 }
            });

            catchNetHandler.ExecuteModelInit();

            ShouldSendInitEvent(1);
        }

        [Test]
        //釋放時, 發送釋放事件
        public void send_release_event_when_release()
        {
            catchNetHandler.Release();

            ShouldSendReleaseEvent(1);
        }

        [Test]
        //成功捕獲時, 發送成功結算事件
        public void send_settle_catch_net_event_when_success_settle()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 }
            });

            catchNetHandler.ExecuteModelInit();

            ShouldSendSettleCatchNetEvent(0);

            CallBeatEventCallback();

            ShouldSendSettleCatchNetEvent(0);

            CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet);

            ShouldSendSettleCatchNetEvent(1);
        }

        [Test]
        //收Beat事件生成捕獲網時, 發送生成捕獲網事件
        public void send_spawn_catch_net_event_when_beat_and_spawn()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 }
            });

            catchNetHandler.ExecuteModelInit();

            ShouldSpawnCatchNet(0);

            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);
        }

        [Test]
        //成功結算時立即生成下一個捕獲網時, 發送生成捕獲網事件
        public void send_spawn_catch_net_event_when_success_settle_and_spawn_next()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);

            CallLastSpawnCatchNetSuccessSettle(out CatchNet _);

            ShouldSpawnCatchNet(2);
        }

        #endregion

        #region 生成捕獲網

        [Test]
        //捕獲網上限為1以上且場上捕獲網數量未達上限時, 下次Beat事件會生成捕獲網
        public void spawn_catch_net_when_current_amount_not_reach_limit_and_limit_is_one()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 },
                { 2, 5 }
            });

            catchNetHandler.ExecuteModelInit();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(0);

            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        //捕獲網上限為0, 下次Beat事件不會生成捕獲網
        public void not_spawn_catch_net_when_limit_is_zero(int feverStage)
        {
            GivenCurrentFeverStage(feverStage);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 0 },
                { 1, 3 },
                { 2, 5 }
            });

            catchNetHandler.ExecuteModelInit();

            CurrentCatchNetLimitShouldBe(0);
            CurrentInFieldCatchNetAmountShouldBe(0);

            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(0);
            CurrentInFieldCatchNetAmountShouldBe(0);
        }

        [Test]
        //捕獲網達上限數量時, 收到Beat事件也不會生成捕獲網
        public void not_spawn_catch_net_when_reach_limit()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 },
                { 2, 5 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);

            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);
        }

        [Test]
        //捕獲網成功結算時, 只要當前數量尚未超出上限則會立即生成捕獲網
        public void spawn_catch_net_immediately_when_success_settle_and_current_amount_not_reach_limit()
        {
            GivenCurrentFeverStage(1);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 },
                { 2, 5 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(3);

            CallLastSpawnCatchNetSuccessSettle(out CatchNet _);

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(3);
        }

        [Test]
        //捕獲網成功結算時, 當前數量已超出上限則不會生成捕獲網
        public void not_spawn_catch_net_when_success_settle_and_current_amount_reach_limit()
        {
            GivenCurrentFeverStage(2);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 },
                { 2, 5 }
            });

            catchNetHandler.ExecuteModelInit();

            for (int i = 0; i < 5; i++)
            {
                CallBeatEventCallback();
            }

            CurrentCatchNetLimitShouldBe(5);
            CurrentInFieldCatchNetAmountShouldBe(5);

            GivenCurrentFeverStage(1);
            CallLastSpawnCatchNetSuccessSettle(out CatchNet _);

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(4);

            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(4);
        }

        [Test]
        //捕獲網原本已超出上限, 但下次Beat事件時會更新上限, 更新後若未達上限則會生成捕獲網
        public void spawn_catch_net_when_beat_and_current_amount_not_reach_limit_after_update_limit()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);

            GivenCurrentFeverStage(1);
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(2);
        }

        [Test]
        //捕獲網原本已超出上限, 但下次成功結算會更新上限, 更新後若未達上限則會生成捕獲網
        public void spawn_catch_net_when_success_settle_and_current_amount_not_reach_limit_after_update_limit()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 3 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);

            GivenCurrentFeverStage(1);
            CallLastSpawnCatchNetSuccessSettle(out CatchNet _);

            CurrentCatchNetLimitShouldBe(3);
            CurrentInFieldCatchNetAmountShouldBe(1);
        }

        [Test]
        //釋放之後, 收到Beat事件, 不會生成捕獲網
        public void do_not_spawn_catch_when_beat_after_release()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 2 }
            });

            catchNetHandler.ExecuteModelInit();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);

            catchNetHandler.Release();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);
        }

        [Test]
        //釋放之後, 清除場上捕獲網
        public void clear_in_field_catch_net_when_release()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 2 }
            });

            catchNetHandler.ExecuteModelInit();
            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(2);

            catchNetHandler.Release();

            CurrentInFieldCatchNetAmountShouldBe(0);
        }

        [Test]
        //生成捕獲網時若沒有隱藏中的捕獲網, 會產出新的捕獲網
        public void spawn_new_catch_net_when_no_hidden_catch_net()
        {
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 2 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(1);
            LastSpawnCatchNetStateShouldBe(CatchNetState.Working);

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(2);
        }

        [Test]
        //生成捕獲網時若有隱藏中的捕獲網, 會重新激活該捕獲網
        public void reactivate_hidden_catch_net_when_spawn_catch_net()
        {
            GivenCurrentFeverStage(1);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 },
                { 1, 2 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(2);
            CurrentInFieldCatchNetAmountShouldBe(2);

            GivenCurrentFeverStage(0);
            CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet);

            CurrentCatchNetLimitShouldBe(1);
            CurrentInFieldCatchNetAmountShouldBe(1);
            CatchNetStateShouldBe(lastCatchNet, CatchNetState.SuccessSettle);

            GivenCurrentFeverStage(1);
            CallBeatEventCallback();

            CurrentCatchNetLimitShouldBe(2);
            CurrentInFieldCatchNetAmountShouldBe(2);
            CatchNetStateShouldBe(lastCatchNet, CatchNetState.Working);
        }

        #endregion

        #region 捕獲旗標&結算

        [Test]
        //生成捕獲網時, 若場上沒有分數球, 則從當前Fever階段的旗標權重設定中隨機生成
        public void spawn_catch_net_and_set_random_flag_num_when_no_score_ball()
        {
            GivenCurrentInFieldScoreBallAmount(0);
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 100 }
            });

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 1 },
            });

            catchNetHandler.ExecuteModelInit();

            Dictionary<int, int> tempTargetFlagDict = new Dictionary<int, int>();
            for (int i = 0; i < 100; i++)
            {
                CallBeatEventCallback();

                CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
                if (tempTargetFlagDict.ContainsKey(arg.TargetFlagNumber))
                    tempTargetFlagDict[arg.TargetFlagNumber]++;
                else
                    tempTargetFlagDict[arg.TargetFlagNumber] = 1;
            }

            Assert.IsTrue(tempTargetFlagDict[1] > 0);
            Assert.IsTrue(tempTargetFlagDict[2] > 0);
            Assert.IsTrue(tempTargetFlagDict[3] > 0);
        }

        [Test]
        //生成捕獲網時, 若場上有分數球, 且場上捕獲網也有包含所有分數球的旗標, 則從當前Fever階段的旗標權重設定中隨機生成
        public void spawn_catch_net_and_set_random_flag_num_when_and_all_score_ball_flag_in_catch_net()
        {
            GivenCurrentInFieldScoreBallAmount(0);
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 100 }
            });

            catchNetHandler.ExecuteModelInit();

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 1, 1 }
            });
            CallBeatEventCallback();
            LastSpawnCatchNetTargetFlagShouldBe(1);

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 3, 1 }
            });
            CallBeatEventCallback();
            LastSpawnCatchNetTargetFlagShouldBe(3);

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 7, 1 }
            });
            CallBeatEventCallback();
            LastSpawnCatchNetTargetFlagShouldBe(7);

            GivenCurrentInFieldScoreBallAmount(3);
            GivenInFieldScoreBallContainsFlagNumber(1, true);
            GivenInFieldScoreBallContainsFlagNumber(3, true);
            GivenInFieldScoreBallContainsFlagNumber(7, true);

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 10, 1 },
                { 11, 1 },
                { 12, 1 }
            });
            Dictionary<int, int> tempTargetFlagDict = new Dictionary<int, int>();
            for (int i = 0; i < 100; i++)
            {
                CallBeatEventCallback();

                CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
                if (tempTargetFlagDict.ContainsKey(arg.TargetFlagNumber))
                    tempTargetFlagDict[arg.TargetFlagNumber]++;
                else
                    tempTargetFlagDict[arg.TargetFlagNumber] = 1;
            }

            Assert.IsTrue(tempTargetFlagDict[10] > 0);
            Assert.IsTrue(tempTargetFlagDict[11] > 0);
            Assert.IsTrue(tempTargetFlagDict[12] > 0);
        }

        //生成捕獲網時, 若場上有分數球, 且場上捕獲網中不包含其中一個分數球的旗標, 則生成該遺漏的旗標
        //生成捕獲網時, 若場上有分數球, 且場上捕獲網中不包含其中多個分數球的旗標, 則從遺漏的旗標中隨機生成
        //生成捕獲網時, 若場上有分數球, 且場上沒有捕獲網, 則從所有分數球的旗標中隨機生成

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        //驗證成功捕獲時的獲得分數
        public void verify_get_score_event_when_success_settle(int score)
        {
            GivenScoreWhenSuccessSettle(score);
            GivenCurrentFeverStage(0);
            GivenCatchNetLimitByFeverStageSetting(new Dictionary<int, int>
            {
                { 0, 1 }
            });

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallLastSpawnCatchNetSuccessSettle(out _);

            LastGetScoreEventShouldBe(score);
        }

        #endregion
    }
}