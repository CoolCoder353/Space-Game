using UnityEngine;
using NaughtyAttributes;

namespace Character
{
    [CreateAssetMenu]
    public class Character_Settings : ScriptableObject
    {
        [Foldout("Main")]
        public float speed;
        [Foldout("Main")]
        public float scrollSpeed;

        [Foldout("Movement")]
        public float shiftSpeedMultiplyer;

        [Foldout("Movement")]
        public Vector2 zoomScale;

        [Foldout("Debug")]
        public bool debug;

    }


}