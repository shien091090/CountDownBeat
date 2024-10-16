namespace GameCore
{
    public interface ICatchNet
    {
        int TargetNumber { get; }
        bool TryTriggerCatch(int number);
    }
}