namespace GameCore
{
    public interface ICatchFlagMergeSetting
    {
        CatchFlagMergeResult GetCatchFlagMergeResult(int flagNum, TriggerFlagMergingType newFlagNum);
    }
}