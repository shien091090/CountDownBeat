using UnityEngine;

namespace GameCore
{
    public interface IScoreBallView : IMVPView
    {
        int GetCurrentCountDownValue { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void SetTextColor(Color color);
        void Close();
        void TriggerCatch();
        void CreateBeatEffectPrefab();
        void PlayAnimation(string animKey);
    }
}