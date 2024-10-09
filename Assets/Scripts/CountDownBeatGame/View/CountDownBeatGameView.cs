using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class CountDownBeatGameView : MonoBehaviour, IArchitectureView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;

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