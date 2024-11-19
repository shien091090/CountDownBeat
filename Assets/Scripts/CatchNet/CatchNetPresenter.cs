using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class CatchNetPresenter : ICatchNetPresenter
    {
        public int SpawnPosIndex { get; private set; }

        private readonly IEventRegister eventRegister;

        private ICatchNet model;
        private ICatchNetView view;
        private bool catchEnable;

        public CatchNetPresenter(IEventRegister eventRegister)
        {
            this.eventRegister = eventRegister;
            ClearData();
        }

        public void Init(int spawnPosIndex, CatchNetSpawnFadeInMode fadeInMode)
        {
            SpawnPosIndex = spawnPosIndex;
            catchEnable = false;

            SetCatchNumberPosType(fadeInMode);
            SetEventRegister(true);
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

        public void BindView(ICatchNetView view)
        {
            this.view = view;
        }

        public void BindModel(ICatchNet model)
        {
            this.model = model;
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

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
            }
        }

        private void SetCatchNumberPosType(CatchNetSpawnFadeInMode fadeInMode)
        {
            view.SetCatchNumberPosType(fadeInMode);
        }

        private void ClearData()
        {
            catchEnable = false;
            SpawnPosIndex = -1;
            SetEventRegister(false);
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

        private void OnBeat(BeatEvent eventInfo)
        {
            view.PlayBeatAnimation();
        }
    }
}