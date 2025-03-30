using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public interface IScoreBallView : IMVPView
    {
        int CurrentFlagNumber { get; }
        ComputableCollider ComputableCollider { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetTextColor(Color color);
        void SetFrameColor(int colorNum);
        void SetDirectionFlag(int directionFlagNum);
        void Close();
        void TriggerCatch();
        void CreateBeatEffectPrefab();
        void PlayAnimation(string animKey);
        void HideAllDirectionFlag();
        void TriggerCheckmark();
    }
}