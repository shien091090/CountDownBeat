namespace GameCore
{
    public class FlagChangeResult
    {
        public int FinalFlagNum { get; set; }
        public bool IsChangeSuccess { get; set; }

        public static FlagChangeResult CreateSuccessInstance(int finalFlagNum)
        {
            return new FlagChangeResult(finalFlagNum, true);
        }

        public static FlagChangeResult CreateFailInstance()
        {
            return new FlagChangeResult(changeSuccess: false);
        }

        private FlagChangeResult(int finalFlagNum = -1, bool changeSuccess = false)
        {
            FinalFlagNum = finalFlagNum;
            IsChangeSuccess = true;
        }
    }
}