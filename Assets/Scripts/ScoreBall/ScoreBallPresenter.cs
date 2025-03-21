using SNShien.Common.TesterTools;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private const string ANIM_KEY_BEAT = "score_ball_beat";
        private const string ANIM_KEY_IDLE = "score_ball_idle";

        public int CurrentFlagNumber => model.CurrentFlagNumber;

        private IScoreBall model;
        private IScoreBallView view;
        private IScoreBallTextColorSetting scoreBallTextColorSetting;
        private IBeaterModel beaterModel;
        private ScoreBallRecordTrajectoryState recordTrajectoryState;

        private readonly Debugger debugger = new Debugger("ScoreBallPresenter");

        public void BindView(IMVPView mvpView)
        {
            view = (IScoreBallView)mvpView;
            ClearData();
        }

        public void UnbindView()
        {
            view = null;
            ClearData();
        }

        public void StartDrag()
        {
            model.SetFreezeState(true);

            BeatAccuracyResult detectBeatAccuracyResult = beaterModel.DetectBeatAccuracyCurrentTime();
            switch (detectBeatAccuracyResult.BeatTimingDirection)
            {
                case BeatTimingDirection.Early:
                    recordTrajectoryState = ScoreBallRecordTrajectoryState.StartDragAndWaitForAfterNextBeat;
                    break;

                case BeatTimingDirection.Late:
                    recordTrajectoryState = ScoreBallRecordTrajectoryState.StartDragAndWaitForNextBeat;
                    break;

                default:
                    return;
            }
        }

        public void CrossDirectionFlagWall(TriggerFlagMergingType triggerFlagMergingType)
        {
            model.MergeFlagWith(triggerFlagMergingType);
        }

        public void TriggerCatch()
        {
            model.SuccessSettle();
        }

        public void BindModel(IMVPModel mvpModel)
        {
            model = (IScoreBall)mvpModel;

            SetEventRegister(true);
        }

        public void DragOver()
        {
            model.SetFreezeState(false);
            ResetRecordTrajectoryState();
        }

        public void Init(IBeaterModel beaterModel, IScoreBallTextColorSetting scoreBallTextColorSetting)
        {
            this.beaterModel = beaterModel;
            this.scoreBallTextColorSetting = scoreBallTextColorSetting;
        }

        private Color GetScoreBallTextColor(int countDownValue)
        {
            return scoreBallTextColorSetting.ConvertToColor(countDownValue);
        }

        private void SetEventRegister(bool isListen)
        {
            model.OnInit -= PlayBeatEffect;
            model.OnUpdateState -= UpdateState;
            model.OnUpdateCountDownValue -= UpdateCountDownNumber;
            model.OnScoreBallBeat -= PlayBeatEffect;
            model.OnUpdateCatchFlagNumber -= OnUpdateCatchFlagNumber;

            if (isListen)
            {
                model.OnInit += PlayBeatEffect;
                model.OnUpdateState += UpdateState;
                model.OnUpdateCountDownValue += UpdateCountDownNumber;
                model.OnScoreBallBeat += PlayBeatEffect;
                model.OnUpdateCatchFlagNumber += OnUpdateCatchFlagNumber;
            }
        }

        private void ClearData()
        {
            ResetRecordTrajectoryState();
        }

        private void UpdateCountDownNumber(int value)
        {
            view.SetCountDownNumberText(value.ToString());
            view.SetTextColor(GetScoreBallTextColor(value));
        }

        private void UpdateState(ScoreBallState state)
        {
            if (state != ScoreBallState.None)
                view.PlayAnimation(ANIM_KEY_IDLE);

            switch (state)
            {
                case ScoreBallState.Hide:
                    view.Close();
                    break;
            }
        }

        private void ResetRecordTrajectoryState()
        {
            recordTrajectoryState = ScoreBallRecordTrajectoryState.None;
        }

        private void PlayBeatEffect()
        {
            view.CreateBeatEffectPrefab();
            view.PlayAnimation(ANIM_KEY_BEAT);
        }

        private void OnUpdateCatchFlagNumber(int flagNumber)
        {
            int colorNum = flagNumber > 10 ?
                flagNumber / 10 :
                flagNumber;

            view.SetFrameColor(colorNum);

            if (flagNumber > 10)
            {
                int directionFlagNum = flagNumber % 10;
                view.SetDirectionFlag(directionFlagNum);
            }
            else
                view.HideAllDirectionFlag();
        }
    }
}