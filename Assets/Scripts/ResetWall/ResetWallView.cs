using System;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(TriggerUICrossDetector))]
    public class ResetWallView : MonoBehaviour
    {
        private TriggerUICrossDetector triggerUICrossDetector;

        private void SetEventRegister(bool isListen)
        {
            triggerUICrossDetector.OnTriggerCross -= OnTriggerCross;
            if (isListen)
            {
                triggerUICrossDetector.OnTriggerCross += OnTriggerCross;
            }
        }

        private void Awake()
        {
            triggerUICrossDetector = GetComponent<TriggerUICrossDetector>();
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