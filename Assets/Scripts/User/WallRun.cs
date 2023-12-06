using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace User
{
    public class WallRun : MonoBehaviour
    {
        [SerializeField] private Transform orientation;
        [SerializeField] private float wallDistance = .5f, minimumJumpHeight = 1.5f;
        [SerializeField] private new Camera camera;
        [SerializeField] private bool wallLeft,wallRight;
        [SerializeField] private float wallRunGravity;
        [SerializeField] private float wallJumpForce;
        [SerializeField] private float fov, wallRunFov, wallRunFovTime,cameraTilt,cameraTiltTime;
        [SerializeField] private Transform discParentTransform;
        
        [SerializeField] private Transform discTransform; // The Transform of the disc
        [SerializeField] private float rotationDuration = 0.2f; // Duration for the rotation animation
        [SerializeField] private Vector3 rotationWhenLeft = new Vector3(-15f, -73f, -27f); // Rotation for left
        [SerializeField] private Vector3 rotationWhenRight = Vector3.zero; // Rotation for right (0,0,0)
        
        public float tilt { get; private set; }
        private Rigidbody rb;
        private RaycastHit leftWallHit, rightWallHit;
        public LayerMask wallLayer;
        
        private bool hasHitLeftWall = false;
        private bool hasHitRightWall = false;
        private Vector3 discLeftPos, discRightPos;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            discRightPos = discParentTransform.localPosition;
            discLeftPos = discRightPos + new Vector3(-0.7f, 0f, 0f);
        }
        private void CheckWall()
        
        {
            bool previouslyWallLeft = wallLeft;
            bool previouslyWallRight = wallRight;

            wallLeft = Physics.Raycast(transform.position, -orientation.right,out leftWallHit, wallDistance,wallLayer);
            wallRight = Physics.Raycast(transform.position, orientation.right,out rightWallHit, wallDistance,wallLayer);
            
            if (wallLeft && !previouslyWallLeft)
            {
                hasHitLeftWall = true;
                discParentTransform.DOLocalMove(discRightPos, 0.2f).SetEase(Ease.InOutCirc);
                RotateDiscParent(rotationWhenRight);
                Debug.Log("moving disc to right");
                
            }
            else if (!wallLeft && hasHitLeftWall)
            {
                hasHitLeftWall = false;
                // Optionally reset position or do other actions when leaving the left wall
            }

            // Check if wallRight was hit for the first time
            if (wallRight && !previouslyWallRight)
            {
                hasHitRightWall = true;
                discParentTransform.DOLocalMove(discLeftPos, 0.2f).SetEase(Ease.InOutCirc);
                RotateDiscParent(rotationWhenLeft);
                Debug.Log("moving disc to left");
            }
            else if (!wallRight && hasHitRightWall)
            {
                hasHitRightWall = false;
                // Optionally reset position or do other actions when leaving the right wall
            }
           
        }
        private void RotateDiscParent(Vector3 newRotation)
        {
            discParentTransform.DOLocalRotate(newRotation, rotationDuration).SetEase(Ease.InOutSine);
        }
        private bool CanWallRun()
        {
            return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
        }
        private void StartWallRun()
        {
            rb.useGravity = false;
            rb.AddForce(Vector3.down*wallRunGravity,ForceMode.Force);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

            if (wallLeft)
            {
                tilt = Mathf.Lerp(tilt, -cameraTilt, cameraTiltTime * Time.deltaTime);
            }
            else if (wallRight)
            {
                tilt = Mathf.Lerp(tilt, cameraTilt, cameraTiltTime * Time.deltaTime);
            }
           
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (wallLeft)
                {
                    Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(wallRunJumpDirection * (wallJumpForce * 100),ForceMode.Force);
                }
                else if (wallRight)
                {
                    Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(wallRunJumpDirection * (wallJumpForce * 100),ForceMode.Force);
                }
            }
        }
        private void StopWallRun()
        {
            rb.useGravity = true;
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
            tilt = Mathf.Lerp(tilt, 0, cameraTiltTime * Time.deltaTime);
        }
        private void Update()
        {
            CheckWall();
            if (CanWallRun())
            {
                if (wallLeft)
                {
                    StartWallRun();
                }
                else if (wallRight)
                {
                    StartWallRun();
                }
                else
                {
                    StopWallRun();
                }
            }
            else
            {
                StopWallRun();
            }
        }
    }
}