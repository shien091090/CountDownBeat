using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameCore
{
    [CustomEditor(typeof(CatchNetHandlerView))]
    public class CatchNetHandlerViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            CatchNetHandlerView component = (CatchNetHandlerView)target;
            if (component.IsShowEditorDrawer == false)
                return;
            
            component.CheckCreateRandomSpawnPositionList();

            if (component.RandomSpawnPositionList == null ||
                component.RandomSpawnPositionList.Count == 0)
                return;

            EditorGUI.BeginChangeCheck();

            Dictionary<int, Vector2> newPosDict = new Dictionary<int, Vector2>();
            for (int i = 0; i < component.RandomSpawnPositionList.Count; i++)
            {
                Vector2 newPos = Handles.PositionHandle(component.RandomSpawnPositionList[i], Quaternion.identity);
                newPosDict[i] = newPos;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(component, "Adjust Spawn Position");

                foreach ((int index, Vector2 newPos) in newPosDict)
                {
                    component.SetPos(index, newPos);
                }

                EditorUtility.SetDirty(component);
            }

            foreach (Vector3 pos in component.RandomSpawnPositionList)
            {
                // Handles.DrawSolidDisc(pos, Vector3.forward, 50f);
                Handles.DrawWireDisc(pos, Vector3.forward, 25f, 3);
            }
        }
    }
}