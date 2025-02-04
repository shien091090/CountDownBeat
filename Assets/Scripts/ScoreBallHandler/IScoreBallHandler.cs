using System;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IScoreBallHandler : IArchitectureModel
    {
        event Action OnRelease;
        event Action OnInit;
    }
}