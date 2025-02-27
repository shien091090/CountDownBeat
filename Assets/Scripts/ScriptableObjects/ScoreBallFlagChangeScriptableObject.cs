using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallFlagChangeScriptableObject : SerializedScriptableObject, IScoreBallFlagChangeSetting
    {
        [SerializeField] private List<FlagChangeDefine> settingList;

        public FlagChangeResult GetChangeFlagNumberInfo(int oldFlagNum, int newFlagNum)
        {
            return FlagChangeResult.CreateSuccessInstance(newFlagNum);
        }

        public void SetDefineList(List<FlagChangeDefine> defineList)
        {
            this.settingList = defineList;
        }
    }
}