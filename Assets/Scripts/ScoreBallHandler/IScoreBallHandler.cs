using System;
using System.Collections.Generic;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IScoreBallHandler : IArchitectureModel
    {
        event Action OnRelease;
        event Action OnInit;
        int CurrentInFieldScoreBallAmount { get; }
        List<int> CurrentInFieldScoreBallFlagNumberList { get; }
    }
}