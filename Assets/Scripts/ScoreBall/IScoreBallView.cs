using UnityEngine;

namespace GameCore
{
    public interface IScoreBallView : IMVPView
    {
        int CurrentFlagNumber { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void SetTextColor(Color color);
        void RecordTrajectoryNode();
        void ClearTrajectoryNode();
        void Close();
        void TriggerCatch();
        void CreateBeatEffectPrefab();
        void PlayAnimation(string animKey);
    }
}