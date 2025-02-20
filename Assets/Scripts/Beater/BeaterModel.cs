using System;
using FMOD.Studio;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IAudioManager audioManager;
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IBeaterPresenter presenter;

        private int beatCounter;
        private int totalBeatCounter;
        private float avgBeatInterval;
        private float halfBeatTimeOffset;
        private float currentBeatTiming;

        public void ExecuteModelInit()
        {
            InitPresenter();

            StartStage(appProcessor.CurrentStageSettingContent);
        }

        public void Release()
        {
            ClearData();
            presenter.UnbindModel();
        }

        public void TriggerHalfBeat()
        {
            eventInvoker.SendEvent(new HalfBeatEvent());
            presenter.PlayHalfBeatAnimation();
        }

        public BeatAccuracyResult DetectBeatAccuracyCurrentTime()
        {
            return DetectBeatAccuracy(presenter.CurrentTimer);
        }

        public BeatAccuracyResult DetectBeatAccuracy(float currentTime)
        {
            if (totalBeatCounter == 0)
                return BeatAccuracyResult.CreateInvalidResult();

            //如果當前時間為上一拍時間到半拍之間, 為Late, 如果當前時間為半拍到下一拍時間之間, 為Early
            BeatTimingDirection direction = currentTime < currentBeatTiming + halfBeatTimeOffset ?
                BeatTimingDirection.Late :
                BeatTimingDirection.Early;

            //以beat時間點為基準, 加減半拍時間範圍內, 判斷打擊拍點的準度, 回應準度結果(0~1), 1為最準, 0為最不準, 在beat時間點上為1, 在半拍時間點上為0
            float accuracy = direction == BeatTimingDirection.Late ?
                1f - Mathf.Abs(currentTime - currentBeatTiming) / halfBeatTimeOffset :
                1f - Mathf.Abs(GetNextBeatTiming() - currentTime) / halfBeatTimeOffset;

            return new BeatAccuracyResult(accuracy, direction);
        }

        public float GetNextBeatTiming()
        {
            return (totalBeatCounter + 1) * avgBeatInterval;
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        private void ClearData()
        {
            beatCounter = 0;
            totalBeatCounter = 0;
            avgBeatInterval = 0;
            halfBeatTimeOffset = 0;
            currentBeatTiming = 0;
        }

        private void UpdateBeatTimeInfo()
        {
            if (totalBeatCounter == 0)
                return;

            avgBeatInterval = presenter.CurrentTimer / totalBeatCounter;
            halfBeatTimeOffset = avgBeatInterval / 2;
            currentBeatTiming = presenter.CurrentTimer;
        }

        private void SendBeatEvent()
        {
            beatCounter++;

            bool isCountDownBeat = false;
            if (beatCounter >= appProcessor.CurrentStageSettingContent.CountDownBeatFreq)
            {
                isCountDownBeat = true;
                beatCounter = 0;
            }

            eventInvoker.SendEvent(new BeatEvent(isCountDownBeat));
        }

        private void StartStage(IStageSettingContent stageSetting)
        {
            audioManager
                .PlayWithCallback(stageSetting.AudioKey)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);

            beatCounter = 0;
        }

        private void OnBeat()
        {
            totalBeatCounter++;

#if UNITY_EDITOR
            try
            {
                SendBeatEvent();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorApplication.isPlaying = false;
            }
#else
                SendBeatEvent();
#endif
            UpdateBeatTimeInfo();
            presenter.SetHalfBeatTimeOffset(halfBeatTimeOffset);
            presenter.PlayBeatAnimation();
        }
    }
}