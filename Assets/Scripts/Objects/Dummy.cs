using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace Objects
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            vfx = GetComponentInChildren<VisualEffect>();
        }

        public void DeathVfx(Vector3 hitDirection)
        {
            //meshRenderer.enabled = false;
            hitDirection.y = 0;

            hitDirection = - hitDirection * 5f;
            // Set the gravity direction in the VFX graph
            vfx.SetVector3("GravityForce", hitDirection);
            vfx.Play();
        }
    }
}