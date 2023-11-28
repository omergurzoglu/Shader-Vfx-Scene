using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace User
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private float sensX, sensY;
        [SerializeField] private Transform cam;
        [SerializeField] private Transform orientation;
        [SerializeField] private MoveCamera moveCamera;
        public float tiltIntensity = 0.5f;
        private WallRun wallRun;
        private float mouseX, mouseY;
        private float multiplier = 0.01f;
        private float xRotation, yRotation;

        private void Start()
        {
            wallRun = GetComponent<WallRun>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            MyInput();
            float waveSlice = moveCamera.GetWaveSlice();
            float tilt = waveSlice * tiltIntensity;

            cam.transform.rotation = Quaternion.Euler(xRotation,yRotation,wallRun.tilt+tilt);
            orientation.transform.rotation = Quaternion.Euler(0,yRotation,0);
        }

        private void MyInput()
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * sensX * multiplier;
            xRotation -= mouseY * sensY * multiplier;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
        }
    }
}