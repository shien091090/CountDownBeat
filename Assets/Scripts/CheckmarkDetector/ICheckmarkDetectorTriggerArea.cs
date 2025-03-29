namespace GameCore
{
    public interface ICheckmarkDetectorTriggerArea
    {
        void Show();
        void Hide();
        CheckmarkFirstTriggerAreaType FirstTriggerAreaType();
        CheckmarkSecondTriggerAreaType SecondTriggerAreaType();
    }
}