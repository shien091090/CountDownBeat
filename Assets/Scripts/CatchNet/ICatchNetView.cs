using System;

namespace GameCore
{
    public interface ICatchNetView
    {
        void SetCatchNumber(string catchNumberText);
        void SetCatchNumberPosType(CatchNetSpawnFadeInMode fadeInMode);
        void Close();
        void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode, Action callback);
        void ResetPos();
    }
}