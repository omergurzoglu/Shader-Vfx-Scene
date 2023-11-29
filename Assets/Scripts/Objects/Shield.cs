using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class Shield : MonoBehaviour
    {
        private Material shieldMaterial;
        private MeshRenderer meshRenderer;
        [SerializeField] private float rippleDuration;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            shieldMaterial = meshRenderer.material;
        }

        public IEnumerator RippleCoroutine()
        {
            float currentTime = 0f;

            // Increase amplitude from 0 to 0.1
            while (currentTime < rippleDuration)
            {
                float alpha = currentTime / rippleDuration;
                float amplitude = Mathf.Lerp(0f, 0.1f, alpha);
                shieldMaterial.SetFloat("_Amplitude", amplitude);
                currentTime += Time.deltaTime;
                yield return null;
            }

            // Reset currentTime for the decreasing phase
            currentTime = 0f;

            // Decrease amplitude from 0.1 back to 0
            while (currentTime < rippleDuration)
            {
                float alpha = currentTime / rippleDuration;
                float amplitude = Mathf.Lerp(0.1f, 0f, alpha);
                shieldMaterial.SetFloat("_Amplitude", amplitude);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ToggleShield();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(RippleCoroutine());
            }
        }

        private void ToggleShield()
        {
            meshRenderer.enabled = !meshRenderer.enabled;
        }
    }

   
}