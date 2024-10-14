namespace GameCore
{
    public interface ICatchNetHandlerPresenter
    {
        int CurrentCatchNetCount { get; }
        void SpawnCatchNet();
    }
}