using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(ComputableColliderCrossDetector))]
    public class DirectionFlagWallView : MonoBehaviour
    {
        [Inject] private IEventRegister eventRegister;

        private ComputableColliderCrossDetector crossDetector;
        private ComputableCollider computableCollider;

        private void SetEventRegister(bool isListen)
        {
            crossDetector.OnTriggerCross -= OnTriggerCross;
            eventRegister?.Unregister<ScoreBallOperateEvent>(OnOperateScoreBall);

            if (isListen)
            {
                crossDetector.OnTriggerCross += OnTriggerCross;
                eventRegister?.Register<ScoreBallOperateEvent>(OnOperateScoreBall);
            }
        }

        private void Awake()
        {
            crossDetector = GetComponent<ComputableColliderCrossDetector>();
            computableCollider = GetComponent<ComputableCollider>();
        }

        private void OnOperateScoreBall(ScoreBallOperateEvent eventInfo)
        {
            if (eventInfo.IsStartDrag)
                computableCollider.StartTrackingTarget(eventInfo.Target.ComputableCollider);
            else
                computableCollider.RemoveTrackingTarget();
        }

        private void OnTriggerCross(GameObject target, ComputableColliderCrossDetector.CrossDetectorCondition crossInfo)
        {
            ScoreBallView scoreBallView = target.GetComponent<ScoreBallView>();
            if (scoreBallView != null)
                scoreBallView.CrossDirectionFlagWall(crossInfo.Key);
        }

        private void OnEnable()
        {
            SetEventRegister(true);
        }

        private void OnDisable()
        {
            SetEventRegister(false);
        }
    }
}