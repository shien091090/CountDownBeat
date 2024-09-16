namespace GameCore
{
    public class ScoreBall
    {
        public int StartCountDownValue { get; private set; }
        public int CurrentCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        public void Init(int startCountDownValue)
        {
            StartCountDownValue = startCountDownValue < 0 ?
                0 :
                startCountDownValue;

            CurrentCountDownValue = StartCountDownValue;
            CurrentState = ScoreBallState.InCountDown;
        }
    }
}