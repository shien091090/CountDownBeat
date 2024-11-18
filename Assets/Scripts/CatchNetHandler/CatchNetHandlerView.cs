using System.Collections.Generic;
using System.Linq;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class CatchNetHandlerView : ArchitectureView, ICatchNetHandlerView
    {
        [SerializeField] private GameSettingScriptableObject gameSetting;
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] private List<CatchNetSpawnPos> randomSpawnPosInfoList;

        public List<CatchNetSpawnPos> RandomSpawnPosInfoList => randomSpawnPosInfoList;
        private ICatchNetHandlerPresenter presenter;
        public bool IsShowEditorDrawer => isShowEditorDrawer;

        public List<Vector3> RandomSpawnPositionList
        {
            get
            {
                if (randomSpawnPosInfoList == null || randomSpawnPosInfoList.Count == 0)
                    return new List<Vector3>();
                else
                    return randomSpawnPosInfoList.Select(x => x.Position).ToList();
            }
        }

        public void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex)
        {
            CatchNetSpawnPos posInfo = RandomSpawnPosInfoList[spawnPosIndex];

            CatchNetView catchNet = objectPoolManager.SpawnGameObject<CatchNetView>(GameConst.PREFAB_NAME_CATCH_NET, posInfo.Position);
            catchNet.BindPresenter(catchNetPresenter);
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as ICatchNetHandlerPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
        }

        public void SetPos(int index, Vector2 newPos)
        {
            List<CatchNetSpawnPos> catchNetSpawnPosList = RandomSpawnPosInfoList;
            if (catchNetSpawnPosList == null)
                return;

            if (catchNetSpawnPosList.Count > index)
                catchNetSpawnPosList[index].SetPosition(newPos);
        }

        public void CheckCreateRandomSpawnPositionList()
        {
            if (gameSetting == null)
                return;

            if (randomSpawnPosInfoList.Count > gameSetting.CatchNetLimit)
                return;

            for (int i = randomSpawnPosInfoList.Count; i <= gameSetting.CatchNetLimit; i++)
            {
                randomSpawnPosInfoList.Add(CatchNetSpawnPos.CreateEmptyInstance());
            }
        }
    }
}