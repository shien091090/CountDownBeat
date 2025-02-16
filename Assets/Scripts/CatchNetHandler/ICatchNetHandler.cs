using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface ICatchNetHandler : IArchitectureModel
    {
        event Action OnInit;
        event Action OnRelease;
        event Action<ICatchNetPresenter> OnSettleCatchNet;
        int CurrentCatchNetLimit { get; }
        void SettleCatchNet(ICatchNet catchNet);
    }
}