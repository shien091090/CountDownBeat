using SNShien.Common.AdapterTools;
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
        [Inject] private IDeltaTimeGetter deltaTimeGetter;

        public float MaxHp { get; private set; }

        private readonly Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_HP_BAR_MODEL);

        public float CurrentHp { get; private set; }

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            SetEventRegister(false);
            presenter.UnbindModel();
        }

        public void Init()
        {
            InitData();
            InitPresenter();
            UpdateCurrentHp(CurrentHp);
            SetEventRegister(true);
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
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

        public void UpdateFrame()
        {
            float damage = appProcessor.CurrentStageSettingContent.HpDecreasePerSecond * deltaTimeGetter.deltaTime;
            IncreaseAndUpdateCurrentHp(-damage);
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<GetScoreEvent>(OnGetScoreEvent);

            if (isListen)
            {
                eventRegister.Register<GetScoreEvent>(OnGetScoreEvent);
            }
        }

        private void IncreaseAndUpdateCurrentHp(float increaseValue)
        {
            float newHp = CurrentHp + increaseValue;

            if (newHp > MaxHp)
                newHp = MaxHp;
            else if (newHp <= 0)
                newHp = 0;

            UpdateCurrentHp(newHp);
        }

        private void UpdateCurrentHp(float newHp)
        {
            CurrentHp = newHp;

            presenter.RefreshHp(CurrentHp);

            if (CurrentHp == 0)
                eventInvoker.SendEvent(new GameOverEvent());
        }

        private void OnGetScoreEvent(GetScoreEvent eventInfo)
        {
            IncreaseAndUpdateCurrentHp(appProcessor.CurrentStageSettingContent.HpIncreasePerCatch);
        }
    }
}