using Sirenix.OdinInspector;

namespace GameCore
{
    public class ScoreBallFlagChangeScriptableObject : SerializedScriptableObject, IScoreBallFlagChangeSetting
    {
        public FlagChangeResult GetChangeFlagNumberInfo(int oldFlagNum, int newFlagNum)
        {
            return FlagChangeResult.CreateSuccessInstance(newFlagNum);
        }
    }
}