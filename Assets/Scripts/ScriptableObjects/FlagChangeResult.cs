namespace GameCore
{
    public class FlagChangeResult
    {
        public int ResultFlagNum { get; }
        public bool IsChangeSuccess { get; }

        public static FlagChangeResult CreateSuccessInstance(int resultFlagNum)
        {
            return new FlagChangeResult(resultFlagNum, true);
        }

        public static FlagChangeResult CreateFailInstance()
        {
            return new FlagChangeResult(isChangeSuccess: false);
        }

        private FlagChangeResult(int resultFlagNum = -1, bool isChangeSuccess = false)
        {
            ResultFlagNum = resultFlagNum;
            IsChangeSuccess = isChangeSuccess;
        }
    }
}