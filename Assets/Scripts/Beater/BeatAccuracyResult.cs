namespace GameCore
{
    public class BeatAccuracyResult
    {
        public BeatTimingDirection BeatTimingDirection { get; }
        public float AccuracyValue { get; }

        public static BeatAccuracyResult CreateInvalidResult()
        {
            return new BeatAccuracyResult(0, BeatTimingDirection.Invalid);
        }

        public BeatAccuracyResult(float accuracy, BeatTimingDirection direction)
        {
            AccuracyValue = accuracy;
            BeatTimingDirection = direction;
        }
    }
}