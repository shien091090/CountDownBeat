using System.Collections.Generic;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class CatchNetHandlerView : MonoBehaviour, ICatchNetHandlerView
    {
        [SerializeField] private GameSettingScriptableObject gameSetting;
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] private List<Vector3> randomSpawnPositionList;
        public List<Vector3> RandomSpawnPositionList => randomSpawnPositionList;

        private ICatchNetHandlerPresenter presenter;
        public bool IsShowEditorDrawer => isShowEditorDrawer;

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

        public void CloseView()
        {
            presenter.UnbindView();
        }

        public void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex)
        {
            Vector3 position = RandomSpawnPositionList[spawnPosIndex];

            CatchNetView catchNet = objectPoolManager.SpawnGameObject<CatchNetView>(GameConst.PREFAB_NAME_CATCH_NET, position);
            catchNet.BindPresenter(catchNetPresenter);
        }

        public void SetPos(int index, Vector2 newPos)
        {
            if (RandomSpawnPositionList == null)
                return;

            if (RandomSpawnPositionList.Count > index)
                RandomSpawnPositionList[index] = newPos;
        }

        public void CheckCreateRandomSpawnPositionList()
        {
            if (gameSetting == null)
                return;
            
            if (randomSpawnPositionList.Count > gameSetting.CatchNetLimit)
                return;

            for (int i = randomSpawnPositionList.Count; i <= gameSetting.CatchNetLimit; i++)
            {
                randomSpawnPositionList.Add(Vector3.zero);
            }
        }
    }
}