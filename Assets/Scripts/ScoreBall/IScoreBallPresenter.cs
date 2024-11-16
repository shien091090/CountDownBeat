namespace GameCore
{
    public interface IScoreBallPresenter
    {
        int CurrentCountDownValue { get; }
        void UpdateCountDownNumber(int value);
        void UpdateState(ScoreBallState state);
        void BindView(IScoreBallView view);
        void DragOver();
        void StartDrag();
        void DoubleClick();
        void TriggerCatch();
        void BindModel(IScoreBall model);
        void PlayBeatEffect();
    }
}