namespace GameCore
{
    public interface IScoreBallPresenter
    {
        void UpdateCountDownNumber(int value);
        void UpdateState(ScoreBallState state);
        void BindView(IScoreBallView view);
        void DragOver();
        void StartDrag();
    }
}