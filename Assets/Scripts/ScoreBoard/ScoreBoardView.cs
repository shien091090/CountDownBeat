using TMPro;
using UnityEngine;

namespace GameCore
{
    public class ScoreBoardView : MonoBehaviour, IScoreBoardView
    {
        [SerializeField] private TextMeshProUGUI tmp_currentScore;
        
        private IScoreBoardPresenter presenter;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            presenter = (IScoreBoardPresenter)parameters[0];
            presenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
        }

        public void SetCurrentScoreText(string scoreText)
        {
            tmp_currentScore.text = scoreText;
        }
    }
}