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
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            InitCatchNetPresenterMock();
            Container.Bind<ICatchNetHandlerPresenter>().FromInstance(presenter).AsSingle();

            InitEventHandlerMock();
            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

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

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenCatchNetLimit(10);
        }

        private void InitCatchNetPresenterMock()
        {
            presenter = Substitute.For<ICatchNetHandlerPresenter>();

            catchNetView = Substitute.For<ICatchNetView>();
            presenter.Spawn(Arg.Any<int>()).Returns(catchNetView);

            GivenTryOccupyPosSuccess(0, CatchNetSpawnFadeInMode.FromBottom);
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

        private void GivenCatchNetLimit(int catchNetLimit)
        {
            gameSetting.CatchNetLimit.Returns(catchNetLimit);
        }

        private void GivenSpawnCatchNetFreqSetting(int spawnCatchNetFreq)
        {
            gameSetting.SpawnCatchNetFreq.Returns(spawnCatchNetFreq);
        }

        private void CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet)
        {
            CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
            arg.TryTriggerCatch(new Vector2Int(arg.TargetNumber, arg.TargetNumber));

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

        #region 發送事件

        [Test]
        //初始化時, 發送初始化事件
        public void send_init_event_when_init()
        {
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
            GivenSpawnCatchNetFreqSetting(1);

            catchNetHandler.ExecuteModelInit();

            ShouldSendSettleCatchNetEvent(0);

            CallBeatEventCallback();
            CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet);

            ShouldSendSettleCatchNetEvent(1);
        }

        #endregion

        #region 生成捕獲網

        [Test]
        //每固定次數Beat時, 生成捕獲網
        public void spawn_catch_net_when_beat()
        {
            GivenSpawnCatchNetFreqSetting(3);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            ShouldSpawnCatchNet(0);

            CallBeatEventCallback();
            ShouldSpawnCatchNet(1);
        }

        [Test]
        //釋放之後, 收到Beat事件, 無反應
        public void do_nothing_when_beat_after_release()
        {
            GivenSpawnCatchNetFreqSetting(1);

            catchNetHandler.ExecuteModelInit();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);

            catchNetHandler.Release();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(1);
        }

        [Test]
        //設定生成頻率為0時, 不會生成捕獲網
        public void not_spawn_catch_net_when_freq_is_0()
        {
            GivenSpawnCatchNetFreqSetting(0);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            ShouldSpawnCatchNet(0);
        }

        [Test]
        //捕獲網達上限數量時, 即使達Beat次數門檻也不會生成捕獲網
        public void not_spawn_catch_net_when_reach_limit()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(4);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);
            CurrentInFieldCatchNetAmountShouldBe(4);

            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);
            CurrentInFieldCatchNetAmountShouldBe(4);
        }

        [Test]
        //捕獲網達上限數量後, 若有捕獲網成功結算, 則後續會再生成捕獲網
        public void spawn_catch_net_after_success_settle()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(4);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);
            CurrentInFieldCatchNetAmountShouldBe(4);

            CallLastSpawnCatchNetSuccessSettle(out _);

            CallBeatEventCallback();

            ShouldSpawnCatchNet(5);
            CurrentInFieldCatchNetAmountShouldBe(4);
        }

        [Test]
        //生成捕獲網時若沒有隱藏中的捕獲網, 會產出新的捕獲網
        public void spawn_new_catch_net_when_no_hidden_catch_net()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(10);

            catchNetHandler.ExecuteModelInit();

            CurrentInFieldCatchNetAmountShouldBe(0);
            ShouldSpawnCatchNet(0);

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(1);
            ShouldSpawnCatchNet(1);
            LastSpawnCatchNetStateShouldBe(CatchNetState.Working);

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(2);
            ShouldSpawnCatchNet(2);
        }

        [Test]
        //生成捕獲網時若有隱藏中的捕獲網, 會重新激活該捕獲網
        public void reactivate_hidden_catch_net_when_spawn_catch_net()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(10);

            catchNetHandler.ExecuteModelInit();

            CurrentInFieldCatchNetAmountShouldBe(0);
            ShouldSpawnCatchNet(0);

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(1);
            ShouldSpawnCatchNet(1);

            CallLastSpawnCatchNetSuccessSettle(out CatchNet lastCatchNet);

            CatchNetStateShouldBe(lastCatchNet, CatchNetState.SuccessSettle);

            CallBeatEventCallback();

            CurrentInFieldCatchNetAmountShouldBe(1);
            ShouldSpawnCatchNet(2);
            CatchNetStateShouldBe(lastCatchNet, CatchNetState.Working);
        }

        #endregion

        #region 捕獲數字&結算

        [Test]
        //生成捕獲網, 驗證捕獲數字
        public void spawn_catch_net_then_verify_target_number()
        {
            GivenCatchNetNumberRange(1, 5);
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(100);

            catchNetHandler.ExecuteModelInit();

            List<int> tempTargetNumList = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                CallBeatEventCallback();
                CatchNet arg = (CatchNet)spawnCatchNetEventCallback.ReceivedCalls().Last().GetArguments()[0];
                tempTargetNumList.Add(arg.TargetNumber);
            }

            tempTargetNumList = tempTargetNumList.Distinct().ToList();
            Assert.AreEqual(5, tempTargetNumList.Count);
            for (int i = 1; i <= 5; i++)
            {
                Assert.IsTrue(tempTargetNumList.Contains(i));
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        //驗證成功捕獲時的獲得分數
        public void verify_get_score_event_when_success_settle(int score)
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenScoreWhenSuccessSettle(score);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallLastSpawnCatchNetSuccessSettle(out _);

            LastGetScoreEventShouldBe(score);
        }

        #endregion
    }
}