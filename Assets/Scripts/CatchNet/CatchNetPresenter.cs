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

            SetCatchFlagNumberPosType(fadeInMode);
            PlaySpawnAnimation(fadeInMode, () =>
            {
                catchEnable = true;
            });
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

            SetEventRegister(true);
        }

        public void ColliderTriggerEnter2D(ICollider2DAdapter col)
        {
            if (catchEnable == false)
                return;

            IScoreBallView scoreBall = col.GetComponent<IScoreBallView>();
            if (scoreBall == null)
                return;

            if (model.TryTriggerCatch(scoreBall.CurrentFlagNumber))
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

        private void SetEventRegister(bool isListen)
        {
            model.OnUpdateState -= UpdateState;
            model.OnCatchNetBeat -= PlayBeatEffect;

            if (isListen)
            {
                model.OnUpdateState += UpdateState;
                model.OnCatchNetBeat += PlayBeatEffect;
            }
        }

        private void SetCatchFlagNumberPosType(CatchNetSpawnFadeInMode fadeInMode)
        {
            view.SetCatchFlagNumberPosType(fadeInMode);
        }

        private void ClearData()
        {
            catchEnable = false;
            SpawnPosIndex = -1;
        }

        private void UpdateState(CatchNetState currentState)
        {
            switch (currentState)
            {
                case CatchNetState.Working:
                    RefreshCatchFlagNumber();
                    break;

                case CatchNetState.SuccessSettle:
                    Hide();
                    break;
            }
        }

        private void RefreshCatchFlagNumber()
        {
            view.SetCatchFlagNumber(model.TargetFlagNumber.ToString("N0"));
        }

        private void PlayBeatEffect()
        {
            view.PlayBeatAnimation();
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