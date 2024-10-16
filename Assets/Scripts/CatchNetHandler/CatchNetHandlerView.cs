using System.Collections.Generic;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class CatchNetHandlerView : MonoBehaviour, ICatchNetHandlerView
    {
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] private List<Vector3> randomSpawnPositionList;

        private ICatchNetHandlerPresenter presenter;
        public bool IsShowEditorDrawer => isShowEditorDrawer;
        public List<Vector3> RandomSpawnPositionList => randomSpawnPositionList;

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

        public void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex)
        {
            Vector3 position = randomSpawnPositionList[spawnPosIndex];

            CatchNetView catchNet = objectPoolManager.SpawnGameObject<CatchNetView>(GameConst.PREFAB_NAME_CATCH_NET, position);
            catchNet.BindPresenter(catchNetPresenter);
        }

        public void SetPos(int index, Vector2 newPos)
        {
            if (randomSpawnPositionList == null)
                return;

            if (randomSpawnPositionList.Count > index)
                randomSpawnPositionList[index] = newPos;
        }
    }
}