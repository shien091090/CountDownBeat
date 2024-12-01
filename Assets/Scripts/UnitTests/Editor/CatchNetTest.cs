using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;

namespace GameCore.UnitTests
{
    public class CatchNetTest
    {
        private CatchNet catchNet;
        private ICatchNetHandler catchNetHandler;
        private IEventRegister eventRegister;
        private ICatchNetPresenter presenter;

        private Action<BeatEvent> beatEventCallback;

        [SetUp]
        public void Setup()
        {
            InitEventRegisterMock();
            catchNetHandler = Substitute.For<ICatchNetHandler>();

            catchNet = new CatchNet(catchNetHandler, eventRegister);

            presenter = Substitute.For<ICatchNetPresenter>();
            catchNet.BindPresenter(presenter);
        }

        [Test]
        //初始化時設定捕獲數字, 並切換狀態為"Working"
        public void init_and_switch_state()
        {
            catchNet.Init(10);

            TargetNumberShouldBe(10);
            CurrentStateShouldBe(CatchNetState.Working);
        }

        [Test]
        //當狀態為"Working"時觸發捕獲判斷, 若數字不一致則不做事
        public void do_nothing_when_trigger_catch_and_number_not_match()
        {
            catchNet.Init(10);

            TryTriggerCatchAndShouldSuccess(5, false);

            CurrentStateShouldBe(CatchNetState.Working);
            ShouldSettleCatchNet(0);
        }

        [Test]
        //當狀態不為"Working"時觸發捕獲判斷, 不做事
        public void do_nothing_when_trigger_catch_and_not_working()
        {
            TryTriggerCatchAndShouldSuccess(10, false);

            CurrentStateShouldBe(CatchNetState.None);
            ShouldSettleCatchNet(0);
        }

        [Test]
        //觸發捕獲判斷, 若數字一致則切換狀態為"SuccessSettle", 並發送得分事件
        public void trigger_catch_and_number_match()
        {
            catchNet.Init(10);
            TryTriggerCatchAndShouldSuccess(10, true);

            CurrentStateShouldBe(CatchNetState.SuccessSettle);
            ShouldSettleCatchNet(1);
        }

        [Test]
        //初始化後狀態在"Working", 每次Beat會撥放特效
        public void play_beat_effect_when_working()
        {
            catchNet.Init(10);

            ShouldPlayBeatEffect(0);

            CallBeatEvent();

            ShouldPlayBeatEffect(1);
            CurrentStateShouldBe(CatchNetState.Working);
        }

        [Test]
        //尚未初始化狀態在"None", 每次Beat不做事
        public void on_beat_then_do_nothing_when_not_init()
        {
            CallBeatEvent();

            ShouldPlayBeatEffect(0);
            CurrentStateShouldBe(CatchNetState.None);
        }

        [Test]
        //成功結算狀態轉為"SuccessSettle"後, 每次Beat不做事
        public void on_beat_then_do_nothing_when_success_settle()
        {
            catchNet.Init(10);

            CallBeatEvent();

            ShouldPlayBeatEffect(1);
            CurrentStateShouldBe(CatchNetState.Working);

            TryTriggerCatchAndShouldSuccess(10, true);

            CallBeatEvent();

            ShouldPlayBeatEffect(1);
            CurrentStateShouldBe(CatchNetState.SuccessSettle);
        }

        private void InitEventRegisterMock()
        {
            eventRegister = Substitute.For<IEventRegister>();

            beatEventCallback = null;
            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback = callback;
            });
            
            eventRegister.When(x => x.Unregister(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                beatEventCallback = null;
            });
        }

        private void CallBeatEvent()
        {
            beatEventCallback?.Invoke(new BeatEvent(true));
        }

        private void ShouldPlayBeatEffect(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                presenter.DidNotReceive().PlayBeatEffect();
            else

                presenter.Received(expectedCallTimes).PlayBeatEffect();
        }

        private void TryTriggerCatchAndShouldSuccess(int catchNumber, bool expectedIsSuccess)
        {
            bool tryTriggerCatch = catchNet.TryTriggerCatch(catchNumber);
            Assert.AreEqual(expectedIsSuccess, tryTriggerCatch);
        }

        private void ShouldSettleCatchNet(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                catchNetHandler.DidNotReceive().SettleCatchNet(Arg.Any<ICatchNet>());
            else
                catchNetHandler.Received(expectedCallTimes).SettleCatchNet(Arg.Any<ICatchNet>());
        }

        private void CurrentStateShouldBe(CatchNetState expectedState)
        {
            Assert.AreEqual(expectedState, catchNet.CurrentState);
        }

        private void TargetNumberShouldBe(int expectedTargetNumber)
        {
            Assert.AreEqual(expectedTargetNumber, catchNet.TargetNumber);
        }
    }
}