using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class FlagStartChangeDefine
    {
        [SerializeField] private int flagNum;
        [SerializeField] private List<FlagChangeResultDefine> combineDefines;

        public int FlagNum => flagNum;

        public FlagStartChangeDefine(int flagNum)
        {
            this.flagNum = flagNum;
        }

        public bool TryChangeFlagNumberTo(int newFlagNum, out int resultFlagNum)
        {
            resultFlagNum = -1;
            FlagChangeResultDefine match = combineDefines.FirstOrDefault(x => x.NewFlagNum == newFlagNum);

            if (match == null)
                return false;
            else
            {
                resultFlagNum = match.ResultFlagNum;
                return true;
            }
        }

        public void AddFlagChangeResultDefine(int newFlagNum, int resultFlagNum)
        {
            if (combineDefines == null)
                combineDefines = new List<FlagChangeResultDefine>();

            FlagChangeResultDefine match = combineDefines.FirstOrDefault(x => x.NewFlagNum == newFlagNum);

            if (match == null)
                combineDefines.Add(new FlagChangeResultDefine(newFlagNum, resultFlagNum));
            else
                match.SetResultFlagNum(resultFlagNum);
        }
    }
}