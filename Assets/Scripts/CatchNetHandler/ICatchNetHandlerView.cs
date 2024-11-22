using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetHandlerView
    {
        List<CatchNetSpawnPos> RandomSpawnPosInfoList { get; }
        void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex);
        ICatchNetView Spawn(int spawnPosIndex);
        void PlayCatchSuccessEffect(Vector3 position);
    }
}