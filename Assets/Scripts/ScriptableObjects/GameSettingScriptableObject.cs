using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : ScriptableObject, IGameSetting
    {
        [SerializeField] private int scoreBallStartCountDownValue;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
    }
}