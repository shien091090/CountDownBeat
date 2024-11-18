using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchNetSpawnPos
    {
        [SerializeField] private CatchNetSpawnFadeInMode fadeInMode;
        [SerializeField] private Vector3 position;

        public CatchNetSpawnFadeInMode FadeInMode => fadeInMode;
        public Vector3 Position => position;

        public static CatchNetSpawnPos CreateEmptyInstance()
        {
            return new CatchNetSpawnPos(-1, Vector3.zero, CatchNetSpawnFadeInMode.None);
        }

        private CatchNetSpawnPos(int index, Vector3 pos, CatchNetSpawnFadeInMode fadeInMode)
        {
            this.position = pos;
            this.fadeInMode = fadeInMode;
        }

        public void SetPosition(Vector2 newPos)
        {
            this.position = newPos;
        }
    }
}