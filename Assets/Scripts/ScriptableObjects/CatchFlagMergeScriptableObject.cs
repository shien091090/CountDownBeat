using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class CatchFlagMergeScriptableObject : SerializedScriptableObject, ICatchFlagMergeSetting
    {
        [SerializeField] private CatchFlagSetting setting;

        public CatchFlagMergeResult GetCatchFlagMergeResult(int flagNum, TriggerFlagMergingType newFlagNum)
        {
            return setting.GetCatchFlagMergeResult(flagNum, newFlagNum);
        }
    }
}