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

        public void Release()
        {
            SetEventRegister(false);
            presenter.UnbindModel();
        }

        private void Init()
        {
            SetEventRegister(true);
            presenter.BindModel(this);
            OpenView();
            UpdateCurrentScore(0);
        }

        private void UpdateCurrentScore(long score)
        {
            currentScore = score;
            presenter.RefreshScoreDisplay(currentScore);
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<GetScoreEvent>(OnGetScoreEvent);

            if (isListen)
            {
                eventRegister.Register<GetScoreEvent>(OnGetScoreEvent);
            }
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