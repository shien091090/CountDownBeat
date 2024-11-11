namespace GameCore
{
    public interface IScoreBoardPresenter
    {
        void RefreshScoreDisplay(long currentScore);
        void BindModel(IScoreBoardModel model);
        void BindView(ScoreBoardView view);
        void OpenView();
        void UnbindView();
        void UnbindModel();
    }
}