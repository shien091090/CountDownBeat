using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : ScriptableObject, IGameSetting
    {
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private float beatTimeThreshold;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public float BeatTimeThreshold => beatTimeThreshold;
    }
}