using System;
using DG.Tweening;
using UnityEngine;

namespace Objects
{
    public class TurretBase : MonoBehaviour
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private float duration = 5f; // Duration for one complete circle
        public bool onMotion;

        private void Start()
        {
            if (onMotion)
            {
                Vector3[] path = GenerateCirclePath(radius, transform.position);
                transform.DOPath(path, duration, PathType.CatmullRom)
                    .SetOptions(true)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
            }
           
        }

        private Vector3[] GenerateCirclePath(float radius, Vector3 center)
        {
            Vector3[] path = new Vector3[4];
            path[0] = center + new Vector3(radius, 0, 0);
            path[1] = center + new Vector3(0, 0, radius);
            path[2] = center + new Vector3(-radius, 0, 0);
            path[3] = center + new Vector3(0, 0, -radius);

            return path;
        }
    }
}