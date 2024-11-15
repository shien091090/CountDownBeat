using SNShien.Common.MonoBehaviorTools;
using TMPro;
using UnityEngine;

namespace GameCore
{
    public class ScoreBoardView : ArchitectureView, IScoreBoardView
    {
        [SerializeField] private TextMeshProUGUI tmp_currentScore;

        private IScoreBoardPresenter presenter;

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = (IScoreBoardPresenter)parameters[0];
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        public void SetCurrentScoreText(string scoreText)
        {
            tmp_currentScore.text = scoreText;
        }
    }
}