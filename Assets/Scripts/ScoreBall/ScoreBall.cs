using System;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;

namespace GameCore
{
    public class ScoreBall : IScoreBall
    {
        public int CurrentFlagNumber { get; private set; }

        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;
        private readonly ICatchFlagSetting flagSetting;
        private readonly Debugger debugger = new Debugger("ScoreBall");

        private IScoreBallPresenter presenter;

        public int CurrentCountDownValue { get; private set; }
        public int StartCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }
        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IEventRegister eventRegister, IEventInvoker eventInvoker, ICatchFlagSetting flagSetting)
        {
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
            this.flagSetting = flagSetting;
        }

        public event Action<int> OnUpdateCatchFlagNumber;
        public event Action OnInit;
        public event Action<ScoreBallState> OnUpdateState;
        public event Action<int> OnUpdateCountDownValue;
        public event Action OnScoreBallBeat;
        public event Action OnScoreBallHalfBeat;

        public void SetFreezeState(bool isFreeze)
        {
            UpdateCurrentState(isFreeze ?
                ScoreBallState.Freeze :
                ScoreBallState.InCountDown);
        }

        public void SuccessSettle()
        {
            Hide();
        }

        public void MergeFlagWith(TriggerFlagMergingType triggerFlagMergingType)
        {
            CatchFlagMergeResult result = flagSetting.GetCatchFlagMergeResult(CurrentFlagNumber, triggerFlagMergingType);
            if (result.IsMergeSuccess)
                UpdateCurrentFlagNumber(result.ResultFlagNum);
        }

        public void ResetToBeginning()
        {
            if (CurrentState == ScoreBallState.Hide)
                return;

            UpdateCurrentCountDownValue(StartCountDownValue);
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (IScoreBallPresenter)mvpPresenter;
        }

        public void Init(int startCountDownValue, int catchFlagNumber)
        {
            if (startCountDownValue <= 0)
            {
                StartCountDownValue = 0;
                return;
            }

            StartCountDownValue = startCountDownValue;

            InitData(catchFlagNumber);
            
            OnInit?.Invoke();
        }

        private void InitData(int catchFlagNumber)
        {
            UpdateCurrentFlagNumber(catchFlagNumber);
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);
        }

        public void Reactivate(int newCatchFlagNumber)
        {
            InitData(newCatchFlagNumber);

            OnInit?.Invoke();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);
            eventRegister.Unregister<HalfBeatEvent>(OnHalfBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
                eventRegister.Register<HalfBeatEvent>(OnHalfBeat);
            }
        }

        private void CheckDamageAndHide()
        {
            if (CurrentCountDownValue > 0)
                return;

            Hide();
            eventInvoker.SendEvent(new DamageEvent());
        }

        private void CheckChangeEventRegisterState(ScoreBallState beforeState, ScoreBallState afterState)
        {
            if ((beforeState == ScoreBallState.None || beforeState == ScoreBallState.Hide) &&
                afterState == ScoreBallState.InCountDown)
                SetEventRegister(true);

            if ((beforeState == ScoreBallState.InCountDown || beforeState == ScoreBallState.Freeze) &&
                afterState == ScoreBallState.Hide)
                SetEventRegister(false);
        }

        private void UpdateCurrentFlagNumber(int flagNumber)
        {
            CurrentFlagNumber = flagNumber;
            OnUpdateCatchFlagNumber?.Invoke(CurrentFlagNumber);
        }

        private void UpdateCurrentState(ScoreBallState newState)
        {
            if (CurrentState == newState)
                return;

            CheckChangeEventRegisterState(CurrentState, newState);

            CurrentState = newState;

            OnUpdateState?.Invoke(CurrentState);
        }

        private void UpdateCurrentCountDownValue(int newValue)
        {
            CurrentCountDownValue = newValue;
            OnUpdateCountDownValue?.Invoke(CurrentCountDownValue);
        }

        private void Hide()
        {
            CurrentCountDownValue = 0;
            UpdateCurrentState(ScoreBallState.Hide);
        }

        private void OnBeat(BeatEvent eventInfo)
        {
            OnScoreBallBeat?.Invoke();

            if (IsCountDownInProcess == false ||
                eventInfo.isCountDownBeat == false)
                return;

            UpdateCurrentCountDownValue(CurrentCountDownValue - 1);
            CheckDamageAndHide();
        }

        private void OnHalfBeat(HalfBeatEvent eventInfo)
        {
            OnScoreBallHalfBeat?.Invoke();
        }
    }
}