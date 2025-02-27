namespace GameCore
{
    public class FlagChangeResult
    {
        public int FinalFlagNum { get; }
        public bool IsChangeSuccess { get; }

        public static FlagChangeResult CreateSuccessInstance(int finalFlagNum)
        {
            return new FlagChangeResult(finalFlagNum, true);
        }

        public static FlagChangeResult CreateFailInstance()
        {
            return new FlagChangeResult(isChangeSuccess: false);
        }

        private FlagChangeResult(int finalFlagNum = -1, bool isChangeSuccess = false)
        {
            FinalFlagNum = finalFlagNum;
            IsChangeSuccess = isChangeSuccess;
        }
    }
}