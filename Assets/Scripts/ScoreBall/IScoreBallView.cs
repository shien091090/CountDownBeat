using UnityEngine;

namespace GameCore
{
    public interface IScoreBallView : IMVPView
    {
        int CurrentFlagNumber { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetTextColor(Color color);
        void SetFrameColor(Color color);
        void RecordTrajectoryNode();
        void ClearTrajectoryNode();
        void Close();
        void TriggerCatch();
        void CreateBeatEffectPrefab();
        void PlayAnimation(string animKey);
    }
}