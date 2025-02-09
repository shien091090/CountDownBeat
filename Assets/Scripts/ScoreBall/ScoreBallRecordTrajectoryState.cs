namespace GameCore
{
    public enum ScoreBallRecordTrajectoryState
    {
        None,
        StartDragAndWaitForNextBeat,
        StartDragAndWaitForAfterNextBeat,
        BypassNextBeat,
        WaitForNextBeatToRecordSecondNode,
        WaitForNextHalfBeatToRecordThirdNode,
    }
}