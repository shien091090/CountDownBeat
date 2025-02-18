using System;

namespace GameCore
{
    public interface ICatchNet : IMVPModel
    {
        event Action<CatchNetState> OnUpdateState;
        event Action OnCatchNetBeat;
        event Action<int> OnUpdateCatchFlagNumber;
        int CatchFlagNumber { get; }
        bool TryTriggerCatch(int flagNumber);
    }
}