using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchFlagSetting
    {
        [SerializeField] private List<CatchFlagDefine> defineList;

        public CatchFlagMergeResult GetCatchFlagMergeResult(int flagNum, TriggerFlagMergingType triggerFlagMergingType)
        {
            if (defineList == null || defineList.Count == 0)
                return CatchFlagMergeResult.CreateFailInstance();

            CatchFlagDefine matchByAnyFlag = defineList.FirstOrDefault(x => x.AnyFlag);
            CatchFlagDefine matchBySpecificFlag = defineList.FirstOrDefault(x => x.FlagNum == flagNum);

            CatchFlagMergeDefine anyFlagAnyTypeMergeDefine = null;
            CatchFlagMergeDefine anyFlagSpecificTypeMergeDefine = null;
            CatchFlagMergeDefine specificFlagAnyTypeMergeDefine = null;
            CatchFlagMergeDefine specificFlagSpecificTypeMergeDefine = null;
            if (matchByAnyFlag != null)
            {
                List<CatchFlagMergeDefine> matchMergeDefineListByAnyFlag = matchByAnyFlag.GetMatchMergeDefineList(triggerFlagMergingType);
                anyFlagAnyTypeMergeDefine = matchMergeDefineListByAnyFlag.FirstOrDefault(x => x.TriggerFlagMergingType == TriggerFlagMergingType.Any);
                anyFlagSpecificTypeMergeDefine = matchMergeDefineListByAnyFlag.FirstOrDefault(x => x.TriggerFlagMergingType == triggerFlagMergingType);
            }

            if (matchBySpecificFlag != null)
            {
                List<CatchFlagMergeDefine> matchMergeDefineListBySpecificFlag = matchBySpecificFlag.GetMatchMergeDefineList(triggerFlagMergingType);
                specificFlagAnyTypeMergeDefine = matchMergeDefineListBySpecificFlag.FirstOrDefault(x => x.TriggerFlagMergingType == TriggerFlagMergingType.Any);
                specificFlagSpecificTypeMergeDefine = matchMergeDefineListBySpecificFlag.FirstOrDefault(x => x.TriggerFlagMergingType == triggerFlagMergingType);
            }

            if (anyFlagAnyTypeMergeDefine != null)
                return CatchFlagMergeResult.CreateSuccessInstance(anyFlagAnyTypeMergeDefine.ResultFlagNum);

            if (specificFlagAnyTypeMergeDefine != null)
                return CatchFlagMergeResult.CreateSuccessInstance(specificFlagAnyTypeMergeDefine.ResultFlagNum);

            if (anyFlagSpecificTypeMergeDefine != null)
                return CatchFlagMergeResult.CreateSuccessInstance(anyFlagSpecificTypeMergeDefine.ResultFlagNum);

            if (specificFlagSpecificTypeMergeDefine != null)
                return CatchFlagMergeResult.CreateSuccessInstance(specificFlagSpecificTypeMergeDefine.ResultFlagNum);

            return CatchFlagMergeResult.CreateFailInstance();
        }

        public void SetDefineList(List<CatchFlagDefine> newSettingList)
        {
            defineList = newSettingList;
        }
    }
}