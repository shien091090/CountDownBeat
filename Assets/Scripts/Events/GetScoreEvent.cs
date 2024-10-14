using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class GetScoreEvent : IArchitectureEvent
    {
        public int Score { get; }

        public GetScoreEvent(int score)
        {
            Score = score;
        }
    }
}