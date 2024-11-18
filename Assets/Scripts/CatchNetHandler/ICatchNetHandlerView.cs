using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetHandlerView
    {
        void Spawn(ICatchNetPresenter catchNetPresenter, int spawnPosIndex);
        List<CatchNetSpawnPos> RandomSpawnPosInfoList { get; }
    }
}