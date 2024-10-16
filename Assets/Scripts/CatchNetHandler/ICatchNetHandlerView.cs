using System.Collections.Generic;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetHandlerView : IArchitectureView
    {
        void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex);
        List<Vector3> RandomSpawnPositionList { get; }
    }
}