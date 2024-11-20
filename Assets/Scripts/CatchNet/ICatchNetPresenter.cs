using System;
using SNShien.Common.AdapterTools;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetPresenter : ICollider2DHandler
    {
        event Action<ICatchNetPresenter> OnSuccessCatch;
        int SpawnPosIndex { get; }
        Vector3 Position { get; }
        void Init(int spawnPosIndex, CatchNetSpawnFadeInMode fadeInMode);
        void UpdateState(CatchNetState currentState);
        void RefreshCatchNumber();
        void BindView(ICatchNetView view);
        void BindModel(ICatchNet model);
    }
}