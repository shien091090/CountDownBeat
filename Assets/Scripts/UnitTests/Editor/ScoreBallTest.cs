using System;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;

namespace GameCore.UnitTests
{
    public class ScoreBallTest
    {
        private ScoreBall scoreBall;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private Action<BeatEvent> beatEventCallback;

        [SetUp]
        public void Setup()
        {
            InitEventHandlerMock();

            scoreBall = new ScoreBall(eventRegister, eventInvoker);
        }

        [Test]
        //設定初始倒數數字並切換狀態為"InCountDown"
        public void init_and_switch_state()
        {
            scoreBall.Init(20);

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //設定初始化時, 監聽事件
        public void init_and_register_event()
        {
            scoreBall.Init(20);

            ShouldRegisterBeatEvent();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        //設定初始倒數數字, 若數字小於0則不做事
        public void init_with_negative_value(int startCountDownValue)
        {
            scoreBall.Init(startCountDownValue);

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.None);
        }

        [Test]
        //收到Beat事件時, 若為倒數拍點, 則倒數數字減一
        public void count_down_when_receive_beat_event_and_is_count_down_beat()
        {
            scoreBall.Init(20);

            CallBeatEventCallback(true);

            CurrentCountDownValueShouldBe(19);
        }

        [Test]
        //收到Beat事件時, 若不是倒數拍點, 則不做事
        public void do_nothing_when_receive_beat_event_and_not_count_down_beat()
        {
            scoreBall.Init(20);

            CallBeatEventCallback(false);

            CurrentCountDownValueShouldBe(20);
        }

        [Test]
        //收到Beat事件時, 倒數數字減至0, 發送Damage事件並切換狀態為"Hide"
        public void count_down_to_zero_and_send_damage_event()
        {
            scoreBall.Init(1);

            CallBeatEventCallback();

            ShouldSendDamageEvent();
            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        //拖曳時, 凍結倒數數字並切換狀態"Freeze"
        public void set_freeze_state_and_stop_count_down()
        {
            scoreBall.Init(20);
            scoreBall.SetFreezeState(true);

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.Freeze);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.Freeze);
        }

        [Test]
        //拖曳時, 通知Presenter更新狀態(inCountDown -> Freeze)
        public void update_state_when_set_freeze()
        {
            scoreBall.Init(20);
            scoreBall.SetFreezeState(true);

            ShouldPresenterUpdateState(1, ScoreBallState.Freeze);
        }

        [Test]
        //成功結算, 切換狀態為"Hide"
        public void success_settle_and_hide()
        {
            scoreBall.Init(10);
            scoreBall.SuccessSettle();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        //取消拖曳狀態, 切換狀態為"InCountDown", 數字繼續倒數
        public void cancel_freeze_then_continue_count_down()
        {
            scoreBall.Init(10);
            scoreBall.SetFreezeState(true);

            CallBeatEventCallback();

            scoreBall.SetFreezeState(false);

            CurrentCountDownValueShouldBe(10);
            CurrentStateShouldBe(ScoreBallState.InCountDown);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(9);
        }

        [Test]
        //重設倒數數字, 數字恢復到起始值
        public void reset_count_down_value()
        {
            scoreBall.Init(10);

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(7);

            scoreBall.ResetToBeginning();

            CurrentCountDownValueShouldBe(10);
        }

        [Test]
        //重設倒數數字, 若狀態為"Hide"則不做事
        public void do_not_reset_count_down_value_when_hide()
        {
            scoreBall.Init(10);
            scoreBall.SuccessSettle();

            scoreBall.ResetToBeginning();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        private void InitEventHandlerMock()
        {
            beatEventCallback = null;

            eventInvoker = Substitute.For<IEventInvoker>();
            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback = callback;
            });
        }

        private void CallBeatEventCallback(bool isCountDownBeat = true)
        {
            beatEventCallback.Invoke(new BeatEvent(isCountDownBeat));
        }

        private void ShouldPresenterUpdateState(int expectedCallTimes, ScoreBallState expectedNewState)
        {
            // presenter.Received(expectedCallTimes).UpdateState(expectedNewState);
        }

        private void ShouldSendDamageEvent(int expectedCallTimes = 1)
        {
            eventInvoker.Received(expectedCallTimes).SendEvent(Arg.Any<DamageEvent>());
        }

        private void ShouldRegisterBeatEvent()
        {
            eventRegister.Received().Unregister(Arg.Any<Action<BeatEvent>>());
            eventRegister.Received().Register(Arg.Any<Action<BeatEvent>>());
        }

        private void CurrentStateShouldBe(ScoreBallState expectedState)
        {
            ScoreBallState currentState = scoreBall.CurrentState;
            Assert.AreEqual(expectedState, currentState);
        }

        private void CurrentCountDownValueShouldBe(int expectedCurrentCountDownValue)
        {
            int currentCountDownValue = scoreBall.CurrentCountDownValue;
            Assert.AreEqual(expectedCurrentCountDownValue, currentCountDownValue);
        }
    }
}