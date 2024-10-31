using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    public class HpBarModel
    {
        [Inject] private IGameSetting gameSetting;
        [Inject] private IBeaterModel beaterModel;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;

        public float CurrentHp { get; private set; }
        public float MaxHp { get; private set; }

        public void Init()
        {
            if (beaterModel.CurrentStageSettingContent == null)
                throw new System.NullReferenceException();

            if (gameSetting.HpMax == 0)
                throw new System.Exception();

            CurrentHp = gameSetting.HpMax;
            MaxHp = gameSetting.HpMax;

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);
            eventRegister.Register<BeatEvent>(OnBeatEvent);

            eventRegister.Unregister<GetScoreEvent>(OnGetScoreEvent);
            eventRegister.Register<GetScoreEvent>(OnGetScoreEvent);
        }

        private void OnGetScoreEvent(GetScoreEvent eventInfo)
        {
            CurrentHp += beaterModel.CurrentStageSettingContent.HpIncreasePerCatch;

            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            CurrentHp -= beaterModel.CurrentStageSettingContent.HpDecreasePerBeat;

            if (CurrentHp <= 0)
            {
                CurrentHp = 0;
                eventInvoker.SendEvent(new GameOverEvent());
            }
        }
    }
}