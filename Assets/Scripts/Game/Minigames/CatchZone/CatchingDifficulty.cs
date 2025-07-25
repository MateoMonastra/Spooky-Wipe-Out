using System;
using UnityEngine;

namespace Game.Minigames.CatchZone
{
    [Serializable]
    public class CatchingDifficulty
    {
        public string name;
        public AnimationCurve movementCurve;
        public float moveSpeed = 1f;
        public float increaseRate = 0.25f;
        public float decreaseRate = 0.15f;
    }
}