namespace GameCore
{
    public interface IScoreBallPresenter : IMVPPresenter
    {
        int CurrentCountDownValue { get; }
        void Init(IScoreBallTextColorSetting scoreBallTextColorSetting);
        void UpdateCountDownNumber(int value);
        void UpdateState(ScoreBallState state);
        void DragOver();
        void StartDrag();
        void DoubleClick();
        void TriggerCatch();
        void PlayBeatEffect();
    }
}