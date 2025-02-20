using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallFlagWeightByFeverStageSetting
    {
        [SerializeField] private int feverStage;
        [SerializeField] private List<ScoreBallFlagWeightDefine> flagWeightSettings;
    }
}