using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IHpBarModel : IArchitectureModel
    {
        float MaxHp { get; }
        void UpdateFrame();
        event Action OnRelease;
        event Action OnInit;
        event Action<float> OnRefreshHp;
    }
}