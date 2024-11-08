using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class HpBarModel : IHpBarModel
    {
        [Inject] private IGameSetting gameSetting;
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IHpBarPresenter presenter;

        public float MaxHp { get; private set; }

        private readonly Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_HP_BAR_MODEL);

        public float CurrentHp { get; private set; }

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
            UpdateCurrentHp(CurrentHp);
        }

        private void InitData()
        {
            if (appProcessor.CurrentStageSettingContent == null)
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
            
            debugger.ShowLog($"hashCode: {this.GetHashCode()}, CurrentHp: {CurrentHp}, increaseValue: {increaseValue}");

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
            UpdateCurrentHp(appProcessor.CurrentStageSettingContent.HpIncreasePerCatch);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            UpdateCurrentHp(-appProcessor.CurrentStageSettingContent.HpDecreasePerBeat);
        }
    }
}