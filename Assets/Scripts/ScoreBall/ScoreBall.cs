using System;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace GameCore
{
    public class ScoreBall : IScoreBall
    {
        public int CurrentCountDownValue { get; private set; }

        public Vector2Int PassCountDownValueRange
        {
            get
            {
                if (isExpand)
                {
                    int maxValue = CurrentCountDownValue + 1;
                    if (maxValue > StartCountDownValue)
                        maxValue = StartCountDownValue;

                    int minValue = CurrentCountDownValue - 1;
                    if (minValue < 1)
                        minValue = 1;

                    return new Vector2Int(minValue, maxValue);
                }
                else
                {
                    return new Vector2Int(CurrentCountDownValue, CurrentCountDownValue);
                }
            }
        }

        private readonly IEventRegister eventRegister;
        private readonly IEventInvoker eventInvoker;

        private IScoreBallPresenter presenter;
        private bool isExpand;

        public int StartCountDownValue { get; private set; }
        public ScoreBallState CurrentState { get; private set; }

        private bool IsCountDownInProcess => CurrentState == ScoreBallState.InCountDown;

        public ScoreBall(IEventRegister eventRegister, IEventInvoker eventInvoker)
        {
            this.eventRegister = eventRegister;
            this.eventInvoker = eventInvoker;
        }

        public event Action OnInit;
        public event Action<ScoreBallState> OnUpdateState;
        public event Action<int> OnUpdateCountDownValue;
        public event Action OnScoreBallBeat;

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

        public void Init(int startCountDownValue)
        {
            if (startCountDownValue <= 0)
                return;

            StartCountDownValue = startCountDownValue;
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);

            OnInit?.Invoke();
        }

        public void TriggerExpand()
        {
            if (CurrentState != ScoreBallState.Freeze)
                return;

            UpdateCurrentState(ScoreBallState.FreezeAndExpand);
        }

        public void Reactivate()
        {
            UpdateCurrentCountDownValue(StartCountDownValue);
            UpdateCurrentState(ScoreBallState.InCountDown);

            OnInit?.Invoke();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeat);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeat);
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

        private void UpdateCurrentState(ScoreBallState newState)
        {
            if (CurrentState == newState)
                return;

            CheckChangeEventRegisterState(CurrentState, newState);

            CurrentState = newState;
            isExpand = newState == ScoreBallState.FreezeAndExpand;

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
    }
}