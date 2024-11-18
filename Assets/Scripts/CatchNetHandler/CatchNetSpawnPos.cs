using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchNetSpawnPos
    {
        public int Index { get; }
        public CatchNetSpawnFadeInMode FadeInMode { get; }
        public Vector3 Position { get; set; }

        public static CatchNetSpawnPos CreateEmptyInstance()
        {
            return new CatchNetSpawnPos(-1, Vector3.zero, CatchNetSpawnFadeInMode.None);
        }

        public CatchNetSpawnPos(int index, CatchNetSpawnFadeInMode fadeInMode)
        {
            Index = index;
            FadeInMode = fadeInMode;
        }

        private CatchNetSpawnPos(int index, Vector3 pos, CatchNetSpawnFadeInMode fadeInMode)
        {
            Index = index;
            Position = pos;
            FadeInMode = fadeInMode;
        }

        public void SetPosition(Vector2 newPos)
        {
            Position = newPos;
        }
    }
}