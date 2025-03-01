using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IDirectionFlagWallHandler : IArchitectureModel
    {
        event Action OnInit;
        event Action OnRelease;
    }
}