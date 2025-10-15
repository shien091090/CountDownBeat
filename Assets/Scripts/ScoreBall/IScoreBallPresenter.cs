namespace GameCore
{
    public interface IScoreBallPresenter : IMVPPresenter
    {
        int CurrentFlagNumber { get; }
        void DragOver();
        void StartDrag();
        void CrossDirectionFlagWall(TriggerFlagMergingType triggerFlagMergingType);
        void TriggerCatch();
        void TriggerCheckmark(CheckmarkSecondTriggerAreaType checkmarkType);
    }
}