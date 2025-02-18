using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class ScoreBallFrameColorByFlagScriptableObject : SerializedScriptableObject, IScoreBallFrameColorByFlagSetting
    {
        [SerializeField] private List<ScoreBallFrameColorDefine> frameColorDefines;

        public Color ConvertToColor(int flagNumber)
        {
            foreach (ScoreBallFrameColorDefine define in frameColorDefines)
            {
                if (define.IsValueMatch(flagNumber))
                    return define.FrameColor;
            }

            return default;
        }
    }
}