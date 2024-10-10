namespace GameCore
{
    public interface IScoreBallPresenter
    {
        void UpdateCountDownNumber(int value);
        void UpdateState(ScoreBallState state);
    }
}