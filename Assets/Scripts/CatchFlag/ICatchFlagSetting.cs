namespace GameCore
{
    public interface ICatchFlagSetting
    {
        CatchFlagMergeResult GetCatchFlagMergeResult(int flagNum, TriggerFlagMergingType newFlagNum);
    }
}