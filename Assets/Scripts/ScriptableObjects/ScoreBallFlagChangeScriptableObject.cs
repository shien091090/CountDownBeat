using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallFlagChangeScriptableObject : SerializedScriptableObject, IScoreBallFlagChangeSetting
    {
        [SerializeField] private ScoreBallFlagChangeSetting setting;

        public FlagChangeResult GetChangeFlagNumberInfo(int oldFlagNum, int newFlagNum)
        {
            return setting.GetChangeFlagNumberInfo(oldFlagNum, newFlagNum);
        }
    }
}