namespace GameCore
{
    public enum ScoreBallRecordTrajectoryState
    {
        None,
        StartDrag,
        WaitForNextBeatToRecordSecondNode,
        WaitForNextHalfBeatToRecordThirdNode
    }
}