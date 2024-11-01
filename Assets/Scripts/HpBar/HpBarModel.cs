using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class HpBarModel : IHpBarModel
    {
        [Inject] private IHpBarPresenter presenter;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IBeaterModel beaterModel;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;

        public float CurrentHp { get; private set; }
        public float MaxHp { get; private set; }

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Init()
        {
            InitData();
            InitPresenter();
            RegisterEvent();
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
            // UpdateCurrentHp(CurrentHp);
        }

        private void InitData()
        {
            if (beaterModel.CurrentStageSettingContent == null)
                throw new System.NullReferenceException();

            if (gameSetting.HpMax == 0)
                throw new System.Exception();

            CurrentHp = gameSetting.HpMax;
            MaxHp = gameSetting.HpMax;
        }

        private void UpdateCurrentHp(float increaseValue)
        {
            CurrentHp += increaseValue;

            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
            else if (CurrentHp <= 0)
                CurrentHp = 0;

            presenter.RefreshHp(CurrentHp);

            if (CurrentHp == 0)
                eventInvoker.SendEvent(new GameOverEvent());
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
            UpdateCurrentHp(beaterModel.CurrentStageSettingContent.HpIncreasePerCatch);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            UpdateCurrentHp(-beaterModel.CurrentStageSettingContent.HpDecreasePerBeat);
        }
    }
}