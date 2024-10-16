using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class CatchNetHandlerView : MonoBehaviour, ICatchNetHandlerView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;
        
        private ICatchNetHandlerPresenter presenter;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as ICatchNetHandlerPresenter;
            presenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
        }

        public void Spawn(ICatchNetPresenter catchNetPresenter)
        {
            CatchNetView catchNet = objectPoolManager.SpawnGameObject<CatchNetView>(GameConst.PREFAB_NAME_CATCH_NET);
            catchNet.BindPresenter(catchNetPresenter);
        }
    }

}