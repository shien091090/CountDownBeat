using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class FlagChangeDefine
    {
        [SerializeField] private int flagNum;
        [SerializeField] private int newFlagNum;
        [SerializeField] private int resultFlagNum;

        public int ResultFlagNum => resultFlagNum;
        public int NewFlagNum => newFlagNum;
        public int FlagNum => flagNum;

        public FlagChangeDefine(int flagNum, int newFlagNum, int resultFlagNum)
        {
            this.flagNum = flagNum;
            this.newFlagNum = newFlagNum;
            this.resultFlagNum = resultFlagNum;
        }
    }
}