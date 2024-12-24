using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SNShien.Common.MonoBehaviorTools;
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    public class CatchNetHandlerView : ArchitectureView, ICatchNetHandlerView
    {
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] [ShowIf("isShowEditorDrawer")] private float editorDrawerRadius;
        [SerializeField] private GameSettingScriptableObject gameSetting;
        [SerializeField] private ObjectPoolManager objectPoolManager;
        [SerializeField] private List<CatchNetSpawnPos> randomSpawnPosInfoList;

        public List<CatchNetSpawnPos> RandomSpawnPosInfoList => randomSpawnPosInfoList;
        private ICatchNetHandlerPresenter presenter;

        public ICatchNetView Spawn(int spawnPosIndex)
        {
            CatchNetSpawnPos posInfo = RandomSpawnPosInfoList[spawnPosIndex];
            CatchNetView view =
                objectPoolManager.SpawnGameObjectAndSetPosition<CatchNetView>(GameConst.PREFAB_NAME_CATCH_NET, posInfo.WorldPosition, TransformType.World);

            return view;
        }

        public void PlayCatchSuccessEffect(Vector3 position)
        {
            objectPoolManager.SpawnGameObjectAndSetPosition(GameConst.PREFAB_NAME_CATCH_SUCCESS_EFFECT, position, TransformType.World);
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isShowEditorDrawer == false)
                return;

            List<Vector3> worldPosList = randomSpawnPosInfoList
                .Where(x => x.HasRectTransform)
                .Select(x => x.WorldPosition)
                .ToList();

            foreach (Vector3 pos in worldPosList)
            {
                Handles.DrawWireDisc(pos, Vector3.forward, editorDrawerRadius);
            }
        }
# endif
    }
}