namespace GameCore
{
    public interface IScoreBallFlagChangeSetting
    {
        FlagChangeResult GetChangeFlagNumberInfo(int oldFlagNum, int newFlagNum);
    }
}