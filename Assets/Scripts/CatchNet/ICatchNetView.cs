using System;
using UnityEngine;

namespace GameCore
{
    public interface ICatchNetView : IMVPView
    {
        Vector3 Position { get; }
        void SetCatchFlagNumber(string catchNumberText);
        void SetCatchFlagNumberPosType(CatchNetSpawnFadeInMode fadeInMode);
        void Close();
        void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode, Action callback);
        void PlayBeatAnimation();
        void ResetPos();
    }
}