using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallFlagWeightByFeverStageSetting
    {
        [SerializeField] private int feverStage;
        [SerializeField] private List<ScoreBallFlagWeightDefine> flagWeightSettings;

        public int FeverStage => feverStage;
        public List<ScoreBallFlagWeightDefine> FlagWeightSettings => flagWeightSettings;

        public ScoreBallFlagWeightByFeverStageSetting(int feverStage, Dictionary<int, int> weightSettingDict)
        {
            this.feverStage = feverStage;
            ParseFlagWeightSettings(weightSettingDict);
        }

        private void ParseFlagWeightSettings(Dictionary<int, int> weightSettingDict)
        {
            flagWeightSettings = new List<ScoreBallFlagWeightDefine>();

            if (weightSettingDict == null || weightSettingDict.Count == 0)
                return;

            foreach ((int flagNumber, int weight) in weightSettingDict)
            {
                flagWeightSettings.Add(new ScoreBallFlagWeightDefine(flagNumber, weight));
            }
        }
    }
}