using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class FlagChangeResultDefine
    {
        [SerializeField] private int newFlagNum;
        [SerializeField] private int resultFlagNum;

        public int NewFlagNum => newFlagNum;
        public int ResultFlagNum => resultFlagNum;

        public FlagChangeResultDefine(int newFlagNum, int resultFlagNum)
        {
            this.newFlagNum = newFlagNum;
            this.resultFlagNum = resultFlagNum;
        }

        public void SetResultFlagNum(int newResultFlagNum)
        {
            resultFlagNum = newResultFlagNum;
        }
    }
}