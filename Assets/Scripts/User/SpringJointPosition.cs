using System;
using UnityEngine;

namespace User
{
    public class SpringJointPosition : MonoBehaviour
    {
        private SpringJoint springJoint;
        [SerializeField] private Transform targetPosition;

        private void Awake()
        {
            springJoint = GetComponent<SpringJoint>();
            
        }
        private void Update()
        {
            springJoint.connectedAnchor = targetPosition.position;
        }
    }
}