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
        private Action<BeatEvent> beatEventCallback;

        [SetUp]
        public void Setup()
        {
            InitEventHandlerMock();

            scoreBall = new(eventRegister);
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

        private void InitEventHandlerMock()
        {
            beatEventCallback = null;

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

        //收到Beat事件時, 倒數數字減至0, 發送Damage事件並切換狀態為"Hide"
        //拖曳時, 凍結倒數數字並切換狀態"Freeze"
        //成功結算, 切換狀態為"Hide"
    }
}