using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class BeaterPresenter : IBeaterPresenter
    {
        [Inject] private IViewManager viewManager;

        public float CurrentTimer { get; private set; }

        private readonly Debugger debugger = new Debugger(DebuggerKeyConst.BEATER_PRESENTER);

        private IBeaterModel model;
        private IBeaterView view;
        private float halfBeatTimeOffset;
        private float halfBeatEventTimer;
        private bool isWaitForNextBeat;

        public void SetHalfBeatTimeOffset(float offset)
        {
            bool isSetFirstTime = halfBeatEventTimer == 0 && offset > 0;
            if (isSetFirstTime)
                isWaitForNextBeat = false;

            halfBeatTimeOffset = offset;
        }

        public void UpdateFrame(float deltaTime)
        {
            CurrentTimer += deltaTime;
            halfBeatEventTimer += deltaTime;

            if (isWaitForNextBeat)
                return;

            if (halfBeatEventTimer >= halfBeatTimeOffset)
            {
                halfBeatEventTimer = 0;
                model.TriggerHalfBeat();
                isWaitForNextBeat = true;
            }
        }

        public void BindView(IBeaterView view)
        {
            ClearData();
            this.view = view;
            view.HideAllHint();
        }

        public void UnbindView()
        {
            view = null;
            ClearData();
        }

        public void BindModel(IBeaterModel model)
        {
            this.model = model;
        }

        public void OpenView()
        {
            viewManager.OpenView<BeaterView>(this);
        }

        public void PlayBeatAnimation()
        {
            StartHalfBeatTimer();
            view?.PlayBeatAnimation();
        }

        public void PlayHalfBeatAnimation()
        {
            view?.PlayHalfBeatAnimation();
        }

        public void UnbindModel()
        {
            model = null;
        }

        private void ClearData()
        {
            halfBeatEventTimer = 0;
            CurrentTimer = 0;
            halfBeatTimeOffset = 0;
            isWaitForNextBeat = true;
        }

        private void StartHalfBeatTimer()
        {
            halfBeatEventTimer = 0;
            isWaitForNextBeat = false;
        }
    }
}