using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace User
{
    public class WallRun : MonoBehaviour
    {
        [SerializeField] private Transform orientation;
        [SerializeField] private float wallDistance = .5f, minimumJumpHeight = 1.5f;
        [SerializeField] private Camera camera;
        [SerializeField] private bool wallLeft,wallRight;
        [SerializeField] private float wallRunGravity;
        [SerializeField] private float wallJumpForce;
        [SerializeField] private float fov, wallRunFov, wallRunFovTime,cameraTilt,cameraTiltTime;
        public float tilt { get; private set; }
        private Rigidbody rb;
        private RaycastHit leftWallHit, rightWallHit;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void CheckWall()
        {
            wallLeft = Physics.Raycast(transform.position, -orientation.right,out leftWallHit, wallDistance);
            wallRight = Physics.Raycast(transform.position, orientation.right,out rightWallHit, wallDistance);
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