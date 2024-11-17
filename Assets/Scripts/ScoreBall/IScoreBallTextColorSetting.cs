using UnityEngine;

namespace GameCore
{
    public interface IScoreBallTextColorSetting
    {
        Color ConvertToColor(int countDownValue);
    }
}