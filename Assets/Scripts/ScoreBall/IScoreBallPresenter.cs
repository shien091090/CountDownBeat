namespace GameCore
{
    public interface IScoreBallPresenter : IMVPPresenter
    {
        int CurrentCountDownValue { get; }
        void Init(IScoreBallTextColorSetting scoreBallTextColorSetting);
        void DragOver();
        void StartDrag();
        void CrossResetWall();
        void TriggerCatch();
        void TriggerTrajectoryAnglePass();
    }
}