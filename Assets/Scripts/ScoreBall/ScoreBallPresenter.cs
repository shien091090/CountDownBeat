using UnityEngine;

namespace GameCore
{
    public class ScoreBallPresenter : IScoreBallPresenter
    {
        private const string ANIM_KEY_BEAT = "score_ball_beat";
        private const string ANIM_KEY_IDLE = "score_ball_idle";

        public int CurrentCountDownValue => model.CurrentCountDownValue;

        private IScoreBall model;
        private IScoreBallView view;
        private IScoreBallTextColorSetting scoreBallTextColorSetting;

        public void UpdateCountDownNumber(int value)
        {
            view.SetCountDownNumberText(value.ToString());
            view.SetTextColor(GetScoreBallTextColor(value));
        }

        public void UpdateState(ScoreBallState state)
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

        public void BindView(IMVPView mvpView)
        {
            view = (IScoreBallView)mvpView;
            view.BindPresenter(this);
        }

        public void UnbindView()
        {
            view = null;
        }

        public void StartDrag()
        {
            model.SetFreezeState(true);
        }

        public void DoubleClick()
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
        }

        public void PlayBeatEffect()
        {
            view.CreateBeatEffectPrefab();
            view.PlayAnimation(ANIM_KEY_BEAT);
        }

        public void DragOver()
        {
            model.SetFreezeState(false);
        }

        public void Init(IScoreBallTextColorSetting scoreBallTextColorSetting)
        {
            this.scoreBallTextColorSetting = scoreBallTextColorSetting;
        }

        private Color GetScoreBallTextColor(int countDownValue)
        {
            return scoreBallTextColorSetting.ConvertToColor(countDownValue);
        }
    }
}