using UnityEngine;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private const string ANIM_KEY_BEAT = "score_ball_beat";
        private const string ANIM_KEY_IDLE = "score_ball_idle";

        private const int RECORD_TRAJECTORY_TIMES_LIMIT = 3;

        public Vector2Int CurrentPassCountDownValueRange => model.PassCountDownValueRange;

        private IScoreBall model;
        private IScoreBallView view;
        private IScoreBallTextColorSetting scoreBallTextColorSetting;
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

            recordTrajectoryState = ScoreBallRecordTrajectoryState.StartDrag;
            view.ClearTrajectoryNode();
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

        public void Init(IScoreBallTextColorSetting scoreBallTextColorSetting)
        {
            this.scoreBallTextColorSetting = scoreBallTextColorSetting;
        }

        public void TriggerTrajectoryAnglePass()
        {
            model.TriggerExpand();
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
                case ScoreBallRecordTrajectoryState.StartDrag:
                    view.RecordTrajectoryNode();
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

                case ScoreBallState.FreezeAndExpand:
                    view.SetExpandColor();
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