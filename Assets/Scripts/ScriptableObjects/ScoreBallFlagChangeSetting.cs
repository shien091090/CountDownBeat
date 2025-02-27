using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallFlagChangeSetting
    {
        [SerializeField] private List<FlagStartChangeDefine> settingList;

        public FlagChangeResult GetChangeFlagNumberInfo(int oldFlagNum, int newFlagNum)
        {
            FlagStartChangeDefine match = settingList.FirstOrDefault(x => x.FlagNum == oldFlagNum);
            if (match == null)
                return FlagChangeResult.CreateFailInstance();
            else
            {
                bool tryChangeFlagNumberTo = match.TryChangeFlagNumberTo(newFlagNum, out int resultFlagNum);
                if (tryChangeFlagNumberTo == false)
                    return FlagChangeResult.CreateFailInstance();
                else
                {
                    return FlagChangeResult.CreateSuccessInstance(resultFlagNum);
                }
            }
        }

        public void SetSettingList(List<FlagStartChangeDefine> newSettingList)
        {
            settingList = newSettingList;
        }
    }
}