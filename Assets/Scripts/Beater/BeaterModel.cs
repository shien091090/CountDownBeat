using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IEventInvoker eventInvoker;

        private BeaterPresenter presenter;
        private float beatTimeThreshold;
        private float currentTimer;
        private bool isAlreadyBeatHalfEvent;

        public void ExecuteModelInit()
        {
            Init();

            presenter = new BeaterPresenter(this);
            viewManager.OpenView<BeaterView>(presenter);
        }

        public void UpdatePerFrame()
        {
            if (beatTimeThreshold == 0)
                return;

            currentTimer += deltaTimeGetter.deltaTime;
            if (currentTimer >= beatTimeThreshold)
            {
                currentTimer -= beatTimeThreshold;
                isAlreadyBeatHalfEvent = false;
                
                eventInvoker.SendEvent(new BeatEvent());
                presenter.PlayBeatAnimation();
            }
            else if (currentTimer >= beatTimeThreshold / 2 && isAlreadyBeatHalfEvent == false)
            {
                isAlreadyBeatHalfEvent = true;
                eventInvoker.SendEvent(new HalfBeatEvent());
            }
        }

        private void Init()
        {
            beatTimeThreshold = gameSetting.BeatTimeThreshold;
            currentTimer = 0;
            isAlreadyBeatHalfEvent = false;
        }
    }
}