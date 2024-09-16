namespace GameCore
{
    public class ScoreBall
    {
        public int StartCountDownValue { get; private set; }
        public int CurrentCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        public void Init(int startCountDownValue)
        {
            if (startCountDownValue <= 0)
                return;

            StartCountDownValue = startCountDownValue;
            CurrentCountDownValue = StartCountDownValue;
            CurrentState = ScoreBallState.InCountDown;
        }
    }
}