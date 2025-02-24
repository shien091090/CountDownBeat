namespace GameCore
{
    public interface IScoreBallPresenter : IMVPPresenter
    {
        int CurrentFlagNumber { get; }
        void DragOver();
        void StartDrag();
        void CrossDirectionFlagWall();
        void TriggerCatch();
    }
}