using System;
using UnityEngine;

namespace User
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        
        public float bobbingSpeed = 14f;
        public float bobbingAmount = 0.05f;
        private float defaultPosY = 0;
        private float timer = 0;
        public float tiltAmount = 0.5f; 

        private void Start()
        {
            defaultPosY = cameraPosition.localPosition.y;
        }
        public float GetWaveSlice()
        {
            return Mathf.Sin(timer);
        }
        private void Update()
        {
            float waveslice = 0;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 localPos = cameraPosition.localPosition;
            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                timer = 0;
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer += bobbingSpeed * Time.deltaTime;
                if (timer > Mathf.PI * 2)
                {
                    timer -= (Mathf.PI * 2);
                }
            }
            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;

                localPos.y = defaultPosY + translateChange;
            }
            else
            {
                localPos.y = defaultPosY;
            }

            cameraPosition.localPosition = localPos;

            

            transform.position = cameraPosition.position;
        }
    }
}