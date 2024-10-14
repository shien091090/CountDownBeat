using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : ScriptableObject, IGameSetting
    {
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private float scoreBallSpawnTimeThreshold;
        [SerializeField] private float beatTimeThreshold;
        [SerializeField] private int successSettleScore;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public float ScoreBallSpawnBeatFreq => scoreBallSpawnTimeThreshold;
        public float BeatTimeThreshold => beatTimeThreshold;
        public int SuccessSettleScore => successSettleScore;
    }
}