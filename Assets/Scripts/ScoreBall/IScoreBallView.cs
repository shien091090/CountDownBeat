using UnityEngine;

namespace GameCore
{
    public interface IScoreBallView
    {
        int GetCurrentCountDownValue { get; }
        void Init();
        void SetCountDownNumberText(string text);
        void SetInCountDownColor();
        void SetFreezeColor();
        void SetTextColor(Color color);
        void Close();
        void TriggerCatch();
        void BindPresenter(IScoreBallPresenter presenter);
        void CreateBeatEffectPrefab();
        void PlayAnimation(string animKey);
    }
}