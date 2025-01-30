using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(ComputableColliderCrossDetector))]
    public class ResetWallView : MonoBehaviour
    {
        private ComputableColliderCrossDetector crossDetector;

        private void SetEventRegister(bool isListen)
        {
            crossDetector.OnTriggerCross -= OnTriggerCross;
            if (isListen)
            {
                crossDetector.OnTriggerCross += OnTriggerCross;
            }
        }

        private void Awake()
        {
            crossDetector = GetComponent<ComputableColliderCrossDetector>();
        }

        private void OnTriggerCross(GameObject target)
        {
            ScoreBallView scoreBallView = target.GetComponent<ScoreBallView>();
            if (scoreBallView != null)
                scoreBallView.CrossResetWall();
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