using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class ScoreBoardModel : IScoreBoardModel
    {
        [Inject] private IScoreBoardPresenter presenter;
        [Inject] private IEventRegister eventRegister;

        private long currentScore;

        public void ExecuteModelInit()
        {
            Init();
        }

        private void Init()
        {
            RegisterEvent();
            presenter.BindModel(this);
            OpenView();
            UpdateCurrentScore(0);
        }

        private void UpdateCurrentScore(long score)
        {
            currentScore = score;
            presenter.RefreshScoreDisplay(currentScore);
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<GetScoreEvent>(OnGetScoreEvent);
            eventRegister.Register<GetScoreEvent>(OnGetScoreEvent);
        }

        private void OpenView()
        {
            presenter.OpenView();
        }

        private void OnGetScoreEvent(GetScoreEvent eventInfo)
        {
            UpdateCurrentScore(currentScore + eventInfo.Score);
        }
    }
}