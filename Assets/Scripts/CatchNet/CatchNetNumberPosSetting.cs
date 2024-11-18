using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchNetNumberPosSetting
    {
        [SerializeField] private CatchNetSpawnFadeInMode fadeInMode;
        [SerializeField] private Vector3 localPosition;

        public CatchNetSpawnFadeInMode FadeInMode => fadeInMode;
        public Vector3 LocalPosition => localPosition;
    }
}