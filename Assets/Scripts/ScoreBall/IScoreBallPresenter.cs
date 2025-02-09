using UnityEngine;

namespace GameCore
{
    public interface IScoreBallPresenter : IMVPPresenter
    {
        Vector2Int CurrentPassCountDownValueRange { get; }
        void Init(IBeaterModel beaterModel, IScoreBallTextColorSetting scoreBallTextColorSetting);
        void DragOver();
        void StartDrag();
        void CrossResetWall();
        void TriggerCatch();
        void TriggerTrajectoryAnglePass();
    }
}