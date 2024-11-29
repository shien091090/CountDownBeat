using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

namespace GameCore
{
    public class CatchNetPresenter : ICatchNetPresenter
    {
        public int SpawnPosIndex { get; private set; }
        public Vector3 Position => view?.Position ?? Vector3.zero;

        private ICatchNet model;
        private ICatchNetView view;
        private bool catchEnable;

        public CatchNetPresenter()
        {
            ClearData();
        }

        public void Init(int spawnPosIndex, CatchNetSpawnFadeInMode fadeInMode)
        {
            SpawnPosIndex = spawnPosIndex;
            catchEnable = false;

            SetCatchNumberPosType(fadeInMode);
            PlaySpawnAnimation(fadeInMode, () =>
            {
                catchEnable = true;
            });
        }

        public void UpdateState(CatchNetState currentState)
        {
            if (currentState == CatchNetState.SuccessSettle)
                Hide();
        }

        public void RefreshCatchNumber()
        {
            view.SetCatchNumber(model.TargetNumber.ToString("N0"));
        }

        public void BindView(IMVPView mvpView)
        {
            view = (ICatchNetView)mvpView;
        }

        public void UnbindView()
        {
            view = null;
        }

        public void BindModel(IMVPModel mvpModel)
        {
            model = (ICatchNet)mvpModel;
        }

        public void ColliderTriggerEnter2D(ICollider2DAdapter col)
        {
            if (catchEnable == false)
                return;

            IScoreBallView scoreBall = col.GetComponent<IScoreBallView>();
            if (scoreBall == null)
                return;

            int currentCountDownValue = scoreBall.GetCurrentCountDownValue;
            if (model.TryTriggerCatch(currentCountDownValue))
                scoreBall.TriggerCatch();
        }

        public void ColliderTriggerExit2D(ICollider2DAdapter col)
        {
        }

        public void ColliderTriggerStay2D(ICollider2DAdapter col)
        {
        }

        public void CollisionEnter2D(ICollision2DAdapter col)
        {
        }

        public void CollisionExit2D(ICollision2DAdapter col)
        {
        }

        public void PlayBeatEffect()
        {
            view.PlayBeatAnimation();
        }

        private void SetCatchNumberPosType(CatchNetSpawnFadeInMode fadeInMode)
        {
            view.SetCatchNumberPosType(fadeInMode);
        }

        private void ClearData()
        {
            catchEnable = false;
            SpawnPosIndex = -1;
        }

        private void Hide()
        {
            view.Close();
            ClearData();
        }

        private void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode, Action callback)
        {
            view.ResetPos();
            view.PlaySpawnAnimation(fadeInMode, () =>
            {
                callback?.Invoke();
            });
        }
    }
}