using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class DirectionFlagWallHandlerView : ArchitectureView, IDirectionFlagWallHandlerView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private Transform tf_wallPosRef;

        private IDirectionFlagWallHandlerPresenter presenter;

        public void Spawn()
        {
            DirectionFlagWallView view =
                objectPoolManager.SpawnGameObjectAndSetPosition<DirectionFlagWallView>(PrefabNameConst.DIRECTION_FLAG_WALL, tf_wallPosRef.position, TransformType.World);
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IDirectionFlagWallHandlerPresenter;
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