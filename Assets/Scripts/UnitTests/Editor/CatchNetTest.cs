using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;

namespace GameCore.UnitTests
{
    public class CatchNetTest
    {
        private CatchNet catchNet;
        private IEventInvoker eventInvoker;
        private IGameSetting gameSetting;
        private ICatchNetPresenter presenter;

        [SetUp]
        public void Setup()
        {
            eventInvoker = Substitute.For<IEventInvoker>();
            gameSetting = Substitute.For<IGameSetting>();
            presenter = Substitute.For<ICatchNetPresenter>();

            catchNet = new CatchNet(presenter, eventInvoker, gameSetting);
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
            bool tryTriggerCatch = catchNet.TryTriggerCatch(5);

            ShouldSendGetScoreEvent(0);
            Assert.IsFalse(tryTriggerCatch);
        }

        [Test]
        //當狀態不為"Working"時觸發捕獲判斷, 不做事
        public void do_nothing_when_trigger_catch_and_not_working()
        {
            bool tryTriggerCatch = catchNet.TryTriggerCatch(10);

            CurrentStateShouldBe(CatchNetState.None);
            ShouldSendGetScoreEvent(0);
            Assert.IsFalse(tryTriggerCatch);
        }

        [Test]
        //觸發捕獲判斷, 若數字一致則切換狀態為"SuccessSettle", 並發送得分事件
        public void trigger_catch_and_number_match()
        {
            catchNet.Init(10);
            bool tryTriggerCatch = catchNet.TryTriggerCatch(10);

            CurrentStateShouldBe(CatchNetState.SuccessSettle);
            ShouldSendGetScoreEvent(1);
            Assert.IsTrue(tryTriggerCatch);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        //驗證成功捕獲時的獲得分數
        public void verify_get_score_event_when_success_settle(int score)
        {
            GivenScoreWhenSuccessSettle(score);

            catchNet.Init(10);
            catchNet.TryTriggerCatch(10);

            ShouldSendGetScoreEvent(1);
            LastGetScoreEventShouldBe(score);
        }

        private void GivenScoreWhenSuccessSettle(int score)
        {
            gameSetting.SuccessSettleScore.Returns(score);
        }

        private void LastGetScoreEventShouldBe(int expectedScore)
        {
            IArchitectureEvent eventArg = (IArchitectureEvent)eventInvoker.ReceivedCalls().Last(x => x.GetMethodInfo().Name == "SendEvent").GetArguments()[0];
            GetScoreEvent getScoreEvent = eventArg as GetScoreEvent;
            Assert.AreEqual(expectedScore, getScoreEvent.Score);
        }

        private void ShouldSendGetScoreEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                eventInvoker.DidNotReceive().SendEvent(Arg.Any<GetScoreEvent>());
            else
                eventInvoker.Received(expectedCallTimes).SendEvent(Arg.Any<GetScoreEvent>());
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