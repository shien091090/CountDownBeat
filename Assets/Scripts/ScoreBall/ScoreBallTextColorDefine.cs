using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallTextColorDefine
    {
        [MinMaxSlider(0, "@MaxValue", ShowFields = true)] [SerializeField]
        private Vector2Int valueRange;

        [SerializeField] private Color textColor;

        public int MaxValue { get; set; }
        public Color TextColor => textColor;

        public bool IsValueMatch(int countDownValue)
        {
            return countDownValue >= valueRange.x && countDownValue <= valueRange.y;
        }
    }
}