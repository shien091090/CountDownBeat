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
        private IScoreBallPresenter presenter;

        [SetUp]
        public void Setup()
        {
            InitEventHandlerMock();
            presenter = Substitute.For<IScoreBallPresenter>();

            scoreBall = new ScoreBall(presenter, eventRegister, eventInvoker);
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
        //收到Beat事件時, 倒數數字減一
        public void count_down_when_receive_beat_event()
        {
            scoreBall.Init(20);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(19);
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
        public void freeze_when_drag()
        {
            scoreBall.Init(20);
            scoreBall.DragAndFreeze();

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.Freeze);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.Freeze);
        }

        [Test]
        //拖曳時, 通知Presenter更新狀態(inCountDown -> Freeze)
        public void update_state_when_drag()
        {
            scoreBall.Init(20);
            scoreBall.DragAndFreeze();

            ShouldPresenterUpdateState(1, ScoreBallState.Freeze);
        }
        
        [Test]
        //拖曳時重複刷新狀態, Presenter只更新一次
        public void update_state_once_when_drag()
        {
            scoreBall.Init(20);
            scoreBall.DragAndFreeze();
            scoreBall.DragAndFreeze();

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
        //拖曳後放開, 從"Freeze"切換狀態為"InCountDOwn"

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

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent());
        }

        private void ShouldPresenterUpdateState(int expectedCallTimes, ScoreBallState expectedNewState)
        {
            presenter.Received(expectedCallTimes).UpdateState(expectedNewState);
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