using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace User
{
    public class PlayerBody : MonoBehaviour
    {
        public float moveSpeed;
        public float walkSpeed=4f,sprintSpeed=6f,acceleration=10f;
        private float horizontalMovement;
        private float verticalMovement;
        private Vector3 moveDirection;
        private Rigidbody rb;
        private bool isGrounded;
        private float playerHeight=2f;
        [SerializeField] private Transform orientation;
        [SerializeField] private float jumpForce;
        [SerializeField] private float groundDrag=6f, airDrag=2f;
        [SerializeField] private float airMultiplier=0.4f;
        private float sphereRadius = 0.4f;
        [SerializeField] private LayerMask groundLayer;
        private Vector3 slopeDirection;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Volume volume;
        private MotionBlur motionBlur;
        private ChromaticAberration chromatic;

        private RaycastHit slopeHit;
        private bool isDashing;
        private new Camera camera;
        
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float chromaticTargetIntensity = 1f;
        [SerializeField] private float fovTarget = 90;
        private float defaultChromaticIntensity;
        private float defaultFov;
       


        private void Start()
        {
            camera=Camera.main;
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            volume.profile.TryGet<MotionBlur>(out var blur);
            {
                motionBlur = blur;
            }
            volume.profile.TryGet<ChromaticAberration>(out var chromaticAberration);
            {
                chromatic = chromaticAberration;
            }
            defaultChromaticIntensity = chromatic.intensity.value;
            defaultFov = camera.fieldOfView;
        }

        private void Update()
        {
            MyInput();
            CheckGround();
            ControlDrag();
            ControlSpeed();
            slopeDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        }

        private void ControlSpeed()
        {
            if (Input.GetKey(KeyCode.LeftShift)&& isGrounded)
            {
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration*Time.deltaTime);
            }
            else
            {
                moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Q) && !isDashing)
            {
               Dash();
            }
        }

        private void Dash()
        {
            if (!isDashing)
            { 
                isDashing = true;
                rb.AddForce(orientation.forward*150f,ForceMode.Impulse);
                StartCoroutine(DashMotionBlur());
            }
           
        }

        private IEnumerator DashMotionBlur()
        {
            
            yield return LerpDashValues(chromaticTargetIntensity, fovTarget, dashDuration);

            // Lerp back to default values
            yield return LerpDashValues(defaultChromaticIntensity, defaultFov, dashDuration);
            isDashing = false;

           
        }

        private IEnumerator LerpDashValues(float targetChromatic, float targetFov, float duration)
        {
            float time = 0f;
            float startChromatic = chromatic.intensity.value;
            float startFov = camera.fieldOfView;

            while (time < duration)
            {
                time += Time.deltaTime;
                chromatic.intensity.value = Mathf.Lerp(startChromatic, targetChromatic, time / duration);
                camera.fieldOfView = Mathf.Lerp(startFov, targetFov, time / duration);
                yield return null;
            }
        }






        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position,Vector3.down,out slopeHit,playerHeight/2+0.5f))
            {
                return slopeHit.normal != Vector3.up;
            }
            return false;

        }
        private void CheckGround()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position,sphereRadius , groundLayer);

            if (Input.GetKeyDown(KeyCode.Space) &&  isGrounded)
            {
                Jump();
            }
            
        }

        private void ControlDrag()
        {
            rb.drag = isGrounded ? groundDrag : airDrag;
        }

        private void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up*jumpForce,ForceMode.Impulse);
        }
        private void MyInput()
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");
            moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        }

        private void FixedUpdate()
        {
           Move();
        }

        private void Move()
        {
            if (isGrounded && !OnSlope())
                rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
            else if 
                (!isGrounded) rb.AddForce(moveDirection.normalized * (moveSpeed * airMultiplier), ForceMode.Acceleration);
            else if (OnSlope() && isGrounded)
            {
                rb.AddForce(slopeDirection.normalized * moveSpeed, ForceMode.Acceleration);
            }
            
        }

        public void JumpPad(int force)
        {
            rb.AddForce(Vector3.up*force,ForceMode.Impulse);
        }
    }
}