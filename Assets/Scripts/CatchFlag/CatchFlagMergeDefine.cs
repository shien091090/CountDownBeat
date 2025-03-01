using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchFlagMergeDefine
    {
        [SerializeField] private TriggerFlagMergingType triggerFlagMergingType;
        [SerializeField] private int resultFlagNum;

        public TriggerFlagMergingType TriggerFlagMergingType => triggerFlagMergingType;
        public int ResultFlagNum => resultFlagNum;

        public CatchFlagMergeDefine(TriggerFlagMergingType triggerFlagMergingType, int resultFlagNum)
        {
            this.triggerFlagMergingType = triggerFlagMergingType;
            this.resultFlagNum = resultFlagNum;
        }

        public void SetResultFlagNum(int newResultFlagNum)
        {
            resultFlagNum = newResultFlagNum;
        }
    }
}