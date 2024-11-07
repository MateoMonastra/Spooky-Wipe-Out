using UnityEngine;
using UnityEngine.Serialization;

namespace VacuumCleaner.Modes
{
    [CreateAssetMenu(fileName = "WashFloorModel", menuName = "Cleaner/Tools/WashFloorModel")]
    public class WashFloorModel : ScriptableObject
    {
        [SerializeField] private float washSpeed;
        [SerializeField] private float minScale;
        [SerializeField] private float renderDistance = 5.0f;
        [SerializeField] private LayerMask wallLayer;

        public float WashSpeed
        {
            get => washSpeed;
            set => washSpeed = value;
        }

        public float MinScale
        {
            get => minScale;
            set => minScale = value;
        }

        public float RenderDistance
        {
            get => renderDistance;
            set => renderDistance = value;
        }

        public LayerMask WallLayer
        {
            get => wallLayer;
            set => wallLayer = value;
        }
    }
}