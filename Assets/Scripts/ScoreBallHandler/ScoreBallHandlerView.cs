using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallHandlerView : ArchitectureView, IScoreBallHandlerView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private RandomPositionInRect randomPositionInRect;

        private IScoreBallHandlerPresenter presenter;

        public IScoreBallView Spawn()
        {
            IScoreBallView scoreBallView = objectPoolManager.SpawnGameObjectAndSetPosition<ScoreBallView>(
                GameConst.PREFAB_NAME_SCORE_BALL,
                randomPositionInRect.GetRandomPosition(),
                TransformType.Local);
            
            return scoreBallView;
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IScoreBallHandlerPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }
    }
}