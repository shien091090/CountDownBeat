using System;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetView
    {
        Vector3 Position { get; }
        void SetCatchNumber(string catchNumberText);
        void SetCatchNumberPosType(CatchNetSpawnFadeInMode fadeInMode);
        void Close();
        void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode, Action callback);
        void PlayBeatAnimation();
        void ResetPos();
    }
}