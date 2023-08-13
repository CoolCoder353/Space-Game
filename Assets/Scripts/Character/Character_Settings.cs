using UnityEngine;
using NaughtyAttributes;

namespace Character
{
    [CreateAssetMenu]
    public class Character_Settings : ScriptableObject
    {
        [Foldout("Main")]
        public int speed;

        [Foldout("Movement")]
        public float shiftSpeedMultiplyer;

        [Foldout("Debug")]
        public bool debug;
        public bool de

    }


}