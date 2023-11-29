using UnityEngine;

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

        private RaycastHit slopeHit;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
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