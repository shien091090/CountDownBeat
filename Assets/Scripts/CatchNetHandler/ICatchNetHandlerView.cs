using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetHandlerView
    {
        List<CatchNetSpawnPos> RandomSpawnPosInfoList { get; }
        ICatchNetView Spawn(int spawnPosIndex);
        void PlayCatchSuccessEffect(Vector3 position);
    }
}