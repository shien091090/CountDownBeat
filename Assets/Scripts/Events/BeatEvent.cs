using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class BeatEvent : IArchitectureEvent
    {
        public bool isCountDownBeat;

        public BeatEvent(bool isCountDownBeat)
        {
            this.isCountDownBeat = isCountDownBeat;
        }
    }
}