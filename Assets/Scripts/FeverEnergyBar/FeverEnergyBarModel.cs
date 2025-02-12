using System.Linq;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class FeverEnergyBarModel : IFeverEnergyBarModel
    {
        [Inject] private IBeaterModel beaterModel;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IFeverEnergyBarPresenter presenter;

        private int beatPenaltyCounter;

        private readonly Debugger debugger = new Debugger("FeverEnergyBarModel");

        public float EnergyValue { get; private set; }
        public int CurrentFeverStage { get; private set; }

        public void ExecuteModelInit()
        {
            ClearData();
            UpdateFeverStage();
            SetEventRegister(true);
            InitPresenter();
        }

        public void Release()
        {
            SetEventRegister(false);
            ClearData();
            presenter.UnbindModel();
        }

        public void Hit()
        {
            BeatAccuracyResult beatAccuracyResult = beaterModel.DetectBeatAccuracyCurrentTime();

            int changeValue;
            bool isCorrectHit = gameSetting.AccuracyPassThreshold >= 1 - beatAccuracyResult.AccuracyValue;
            if (isCorrectHit)
            {
                changeValue = gameSetting.FeverEnergyIncrease;
                beatPenaltyCounter = 0;
            }
            else
            {
                changeValue = -gameSetting.FeverEnergyDecrease;
            }

            AddEnergyValue(changeValue);
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        private int GetEnergyBarMaxValue()
        {
            int[] energyBarSetting = gameSetting.FeverEnergyBarSetting;
            return energyBarSetting == null || energyBarSetting.Length == 0 ?
                0 :
                energyBarSetting.Sum();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<HalfBeatEvent>(OnHalfBeatEvent);

            if (isListen)
                eventRegister.Register<HalfBeatEvent>(OnHalfBeatEvent);
        }

        private void AddEnergyValue(int addValue)
        {
            EnergyValue += addValue;
            EnergyValue = Mathf.Clamp(EnergyValue, 0, GetEnergyBarMaxValue());
            UpdateFeverStage();

            debugger.ShowLog($"EnergyValue: {EnergyValue}, CurrentFeverStage: {CurrentFeverStage}", true);
        }

        private void ClearData()
        {
            EnergyValue = 0;
            CurrentFeverStage = 0;
            beatPenaltyCounter = 0;
        }

        private void UpdateFeverStage()
        {
            int[] energyBarSetting = gameSetting.FeverEnergyBarSetting;
            int totalValue = 0;

            for (int i = 0; i < energyBarSetting.Length; i++)
            {
                totalValue += energyBarSetting[i];
                if (EnergyValue <= totalValue)
                {
                    CurrentFeverStage = i;
                    break;
                }
            }
        }

        private void OnHalfBeatEvent(HalfBeatEvent eventInfo)
        {
            beatPenaltyCounter++;

            if (beatPenaltyCounter >= 2)
                AddEnergyValue(-gameSetting.FeverEnergyDecrease);
        }
    }
}