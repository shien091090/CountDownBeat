using SNShien.Common.MonoBehaviorTools;
using Zenject;

namespace GameCore
{
    public class CountDownBeatGameView : IArchitectureView
    {
        [Inject] private IGameObjectPool objectPoolManager;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            ScoreBallPresenter presenter = parameters[0] as ScoreBallPresenter;
            ScoreBallView scoreBall = objectPoolManager.SpawnGameObject<ScoreBallView>(GameConst.PREFAB_NAME_SCORE_BALL);
            scoreBall.BindPresenter(presenter);
        }

        public void ReOpenView(params object[] parameters)
        {
        }
    }
}