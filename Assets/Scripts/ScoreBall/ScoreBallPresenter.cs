using UnityEngine;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private const string ANIM_KEY_BEAT = "score_ball_beat";
        private const string ANIM_KEY_IDLE = "score_ball_idle";

        private const int RECORD_TRAJECTORY_TIMES_LIMIT = 3;

        public int CurrentFlagNumber => model.CurrentFlagNumber;

        private IScoreBall model;
        private IScoreBallView view;
        private IScoreBallTextColorSetting scoreBallTextColorSetting;
        private IBeaterModel beaterModel;
        private ScoreBallRecordTrajectoryState recordTrajectoryState;

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

            view.ClearTrajectoryNode();

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

            CheckRecordTrajectoryNode();
        }

        public void CrossResetWall()
        {
            model.ResetToBeginning();
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
            model.OnScoreBallBeat -= OnScoreBallBeat;
            model.OnScoreBallHalfBeat -= OnScoreBallHalfBeat;

            if (isListen)
            {
                model.OnInit += PlayBeatEffect;
                model.OnUpdateState += UpdateState;
                model.OnUpdateCountDownValue += UpdateCountDownNumber;
                model.OnScoreBallBeat += PlayBeatEffect;
                model.OnScoreBallBeat += OnScoreBallBeat;
                model.OnScoreBallHalfBeat += OnScoreBallHalfBeat;
            }
        }

        private void CheckRecordTrajectoryNode(ScoreBallBeatType beatType = ScoreBallBeatType.None)
        {
            switch (recordTrajectoryState)
            {
                case ScoreBallRecordTrajectoryState.StartDragAndWaitForNextBeat:
                    view.RecordTrajectoryNode();
                    recordTrajectoryState = ScoreBallRecordTrajectoryState.WaitForNextBeatToRecordSecondNode;
                    break;

                case ScoreBallRecordTrajectoryState.StartDragAndWaitForAfterNextBeat:
                    view.RecordTrajectoryNode();
                    recordTrajectoryState = ScoreBallRecordTrajectoryState.BypassNextBeat;
                    break;

                case ScoreBallRecordTrajectoryState.BypassNextBeat:
                    if (beatType == ScoreBallBeatType.Beat)
                        recordTrajectoryState = ScoreBallRecordTrajectoryState.WaitForNextBeatToRecordSecondNode;
                    break;

                case ScoreBallRecordTrajectoryState.WaitForNextBeatToRecordSecondNode:
                    if (beatType == ScoreBallBeatType.Beat)
                    {
                        view.RecordTrajectoryNode();
                        recordTrajectoryState = ScoreBallRecordTrajectoryState.WaitForNextHalfBeatToRecordThirdNode;
                    }

                    break;

                case ScoreBallRecordTrajectoryState.WaitForNextHalfBeatToRecordThirdNode:
                    if (beatType == ScoreBallBeatType.HalfBeat)
                    {
                        view.RecordTrajectoryNode();
                        ResetRecordTrajectoryState();
                    }

                    break;
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
                case ScoreBallState.InCountDown:
                    view.SetInCountDownColor();
                    break;

                case ScoreBallState.Hide:
                    view.Close();
                    break;

                case ScoreBallState.Freeze:
                    view.SetFreezeColor();
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

        private void OnScoreBallHalfBeat()
        {
            CheckRecordTrajectoryNode(ScoreBallBeatType.HalfBeat);
        }

        private void OnScoreBallBeat()
        {
            CheckRecordTrajectoryNode(ScoreBallBeatType.Beat);
        }
    }
}