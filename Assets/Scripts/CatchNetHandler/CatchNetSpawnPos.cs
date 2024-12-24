using UnityEngine;

namespace GameCore
{
    [System.Serializable]
    public class CatchNetSpawnPos
    {
        [SerializeField] private CatchNetSpawnFadeInMode fadeInMode;
        [SerializeField] private RectTransform rectTransform;

        public CatchNetSpawnFadeInMode FadeInMode => fadeInMode;
        public Vector3 LocalPosition => rectTransform.localPosition;
        public Vector3 WorldPosition => rectTransform.position;
        public bool HasRectTransform => rectTransform != null;
    }
}