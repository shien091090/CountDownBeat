using UnityEngine;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private const string ANIM_KEY_BEAT = "score_ball_beat";
        private const string ANIM_KEY_IDLE = "score_ball_idle";

        public Vector2Int CurrentPassCountDownValueRange => model.PassCountDownValueRange;

        private IScoreBall model;
        private IScoreBallView view;
        private IScoreBallTextColorSetting scoreBallTextColorSetting;

        public void BindView(IMVPView mvpView)
        {
            view = (IScoreBallView)mvpView;
        }

        public void UnbindView()
        {
            view = null;
        }

        public void StartDrag()
        {
            model.SetFreezeState(true);
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
            model.OnScoreBallBeat -= RecordTrajectoryNode;
            model.OnScoreBallHalfBeat -= RecordTrajectoryNode;

            if (isListen)
            {
                model.OnInit += PlayBeatEffect;
                model.OnUpdateState += UpdateState;
                model.OnUpdateCountDownValue += UpdateCountDownNumber;
                model.OnScoreBallBeat += PlayBeatEffect;
                model.OnScoreBallBeat += RecordTrajectoryNode;
                model.OnScoreBallHalfBeat += RecordTrajectoryNode;
            }
        }

        private void RecordTrajectoryNode()
        {
            view.RecordTrajectoryNode();
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

        private void PlayBeatEffect()
        {
            view.CreateBeatEffectPrefab();
            view.PlayAnimation(ANIM_KEY_BEAT);
        }
    }
}