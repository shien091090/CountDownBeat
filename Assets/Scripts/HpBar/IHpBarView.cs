using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public interface IHpBarView : IArchitectureView
    {
        void RefreshHpSliderValue(float value);
    }
}