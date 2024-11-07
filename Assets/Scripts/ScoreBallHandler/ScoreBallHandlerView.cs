using System;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class ScoreBallHandlerView : MonoBehaviour, IScoreBallHandlerView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;

        private IScoreBallHandlerPresenter presenter;
        private RandomPositionInRect randomPositionInRect;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IScoreBallHandlerPresenter;
            presenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
        }

        public void CloseView()
        {
            presenter.UnbindView();
        }

        public IScoreBallView Spawn()
        {
            IScoreBallView scoreBallView = objectPoolManager.SpawnGameObject<ScoreBallView>(GameConst.PREFAB_NAME_SCORE_BALL, randomPositionInRect.GetRandomPosition());
            return scoreBallView;
        }

        private void Awake()
        {
            randomPositionInRect = gameObject.GetComponent<RandomPositionInRect>();
        }
    }
}