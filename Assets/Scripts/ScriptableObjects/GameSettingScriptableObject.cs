using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : ScriptableObject, IGameSetting
    {
        [SerializeField] private ScoreBallTextColorSettingScriptableObject scoreBallTextColorSetting;
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private int successSettleScore;
        [SerializeField] private int spawnCatchNetFreq;
        [SerializeField] private int catchNetLimit;
        [SerializeField] private Vector2Int catchNetNumberRange;
        [SerializeField] private float hpMax;
        [SerializeField] private float accuracyPassThreshold;
        [SerializeField] private float feverEnergyIncrease;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public int SuccessSettleScore => successSettleScore;
        public int SpawnCatchNetFreq => spawnCatchNetFreq;
        public int CatchNetLimit => catchNetLimit;
        public Vector2Int CatchNetNumberRange => catchNetNumberRange;
        public float HpMax => hpMax;
        public IScoreBallTextColorSetting ScoreBallTextColorSetting => scoreBallTextColorSetting;
        public float AccuracyPassThreshold => accuracyPassThreshold;
        public float FeverEnergyIncrease => feverEnergyIncrease;
    }
}