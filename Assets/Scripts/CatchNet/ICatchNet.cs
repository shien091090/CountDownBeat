namespace GameCore
{
    public interface ICatchNet : IMVPModel
    {
        int TargetNumber { get; }
        bool TryTriggerCatch(int number);
    }
}