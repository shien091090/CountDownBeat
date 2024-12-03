using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface ICatchNetHandler : IArchitectureModel
    {
        event Action OnInit;
        void SettleCatchNet(ICatchNet catchNet);
        event Action OnRelease;
        event Action<ICatchNetPresenter> OnSettleCatchNet;
    }
}