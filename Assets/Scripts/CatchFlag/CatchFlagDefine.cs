using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchFlagDefine
    {
        [SerializeField] private bool anyFlag;
        [SerializeField] private int flagNum;
        [SerializeField] private List<CatchFlagMergeDefine> mergeDefines;

        public int FlagNum => flagNum;
        public bool AnyFlag => anyFlag;

        public static CatchFlagDefine CreateSpecificFlagInstance(int flagNum)
        {
            return new CatchFlagDefine(flagNum, false);
        }

        public static CatchFlagDefine CreateAnyFlagInstance()
        {
            return new CatchFlagDefine(-1, true);
        }

        private CatchFlagDefine(int flagNum, bool anyFlag)
        {
            this.flagNum = flagNum;
            this.anyFlag = anyFlag;
        }

        public List<CatchFlagMergeDefine> GetMatchMergeDefineList(TriggerFlagMergingType triggerFlagMergingType)
        {
            List<CatchFlagMergeDefine> result = new List<CatchFlagMergeDefine>();
            List<CatchFlagMergeDefine> matchList = mergeDefines
                .Where(x => x.TriggerFlagMergingType == TriggerFlagMergingType.Any || x.TriggerFlagMergingType == triggerFlagMergingType)
                .ToList();

            result.AddRange(matchList);

            return result;
        }

        public void AddCatchFlagMergeDefine(TriggerFlagMergingType triggerFlagMergingType, int resultFlagNum)
        {
            if (mergeDefines == null)
                mergeDefines = new List<CatchFlagMergeDefine>();

            CatchFlagMergeDefine match = mergeDefines.FirstOrDefault(x => x.TriggerFlagMergingType == triggerFlagMergingType);

            if (match == null)
                mergeDefines.Add(new CatchFlagMergeDefine(triggerFlagMergingType, resultFlagNum));
            else
                match.SetResultFlagNum(resultFlagNum);
        }
    }
}