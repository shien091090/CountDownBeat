namespace GameCore
{
    public class CatchFlagMergeResult
    {
        public int ResultFlagNum { get; }
        public bool IsMergeSuccess { get; }

        public static CatchFlagMergeResult CreateSuccessInstance(int resultFlagNum)
        {
            return new CatchFlagMergeResult(resultFlagNum, true);
        }

        public static CatchFlagMergeResult CreateFailInstance()
        {
            return new CatchFlagMergeResult(isMergeSuccess: false);
        }

        private CatchFlagMergeResult(int resultFlagNum = -1, bool isMergeSuccess = false)
        {
            ResultFlagNum = resultFlagNum;
            IsMergeSuccess = isMergeSuccess;
        }
    }
}