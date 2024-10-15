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
        [SerializeField] private int spawnCatchNetFreq;
        [SerializeField] private int catchNetLimit;
        [SerializeField] private Vector2Int catchNetNumberRange;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public float ScoreBallSpawnBeatFreq => scoreBallSpawnTimeThreshold;
        public float BeatTimeThreshold => beatTimeThreshold;
        public int SuccessSettleScore => successSettleScore;
        public int SpawnCatchNetFreq => spawnCatchNetFreq;
        public int CatchNetLimit => catchNetLimit;
        public Vector2Int CatchNetNumberRange => catchNetNumberRange;
    }
}