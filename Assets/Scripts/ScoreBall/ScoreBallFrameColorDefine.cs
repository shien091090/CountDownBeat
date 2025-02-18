using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class ScoreBallFrameColorDefine
    {
        [SerializeField] private int flagNumber;
        [SerializeField] private Color frameColor;
        
        public int FlagNumber => flagNumber;
        public Color FrameColor => frameColor;

        public bool IsValueMatch(int targetFlagNumber)
        {
            return FlagNumber == targetFlagNumber;
        }
    }
}