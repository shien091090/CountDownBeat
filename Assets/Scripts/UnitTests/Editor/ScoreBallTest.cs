using NUnit.Framework;

namespace GameCore.UnitTests
{
    public class ScoreBallTest
    {
        private static ScoreBall scoreBall;

        [SetUp]
        public static void Setup()
        {
            scoreBall = new();
        }

        [Test]
        //設定初始倒數數字並切換狀態為"InCountDown"
        public void init_and_switch_state()
        {
            scoreBall.Init(20);

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.InCountDown);
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

        //設定初始倒數數字, 若數字小於0則不做事
        //收到Beat事件時, 倒數數字減一
        //收到Beat事件時, 倒數數字減至0, 發送Damage事件並切換狀態為"Hide"
        //拖曳時, 凍結倒數數字並切換狀態"Freeze"
        //成功結算, 切換狀態為"Hide"
    }
}