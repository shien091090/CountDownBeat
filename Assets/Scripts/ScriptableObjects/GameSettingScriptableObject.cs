using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    public class GameSettingScriptableObject : SerializedScriptableObject, IGameSetting
    {
        [SerializeField] private ScoreBallTextColorSettingScriptableObject scoreBallTextColorSetting;
        [SerializeField] private ScoreBallFrameColorByFlagScriptableObject scoreBallFrameColorByFlagSetting;
        [SerializeField] private float hpMax;
        [SerializeField] private float accuracyPassThreshold;

        public float HpMax => hpMax;
        public IScoreBallTextColorSetting ScoreBallTextColorSetting => scoreBallTextColorSetting;
        public IScoreBallFrameColorByFlagSetting ScoreBallFrameColorByFlagSetting => scoreBallFrameColorByFlagSetting;
        public float AccuracyPassThreshold => accuracyPassThreshold;
    }
}