using System;
using UnityEngine;

namespace Game.Garbage
{
    public class Trash : MonoBehaviour, IVacuumable
    {
        [SerializeField] private GameObject model;
        public Action<Trash> OnBeingDestroy;

        private void OnEnable()
        {
            GameManager.GetInstance().AddTrash(this);
        }

        public void IsBeingVacuumed(params object[] args)
        {
            var rb = GetComponentInChildren<Rigidbody>();

            var direction = ((Vector3)args[0] - model.transform.position).normalized;
            
            rb.AddForce(direction * (float)args[1], ForceMode.VelocityChange);
        }
    }
}
