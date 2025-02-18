using UnityEngine;

namespace GameCore
{
    public interface IScoreBallFrameColorByFlagSetting
    {
        Color ConvertToColor(int flagNumber);
    }
}