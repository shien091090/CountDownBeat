using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class CatchFlagScriptableObject : SerializedScriptableObject, ICatchFlagSetting
    {
        [SerializeField] private CatchFlagSetting setting;

        public CatchFlagMergeResult GetCatchFlagMergeResult(int flagNum, TriggerFlagMergingType newFlagNum)
        {
            return setting.GetCatchFlagMergeResult(flagNum, newFlagNum);
        }
    }
}