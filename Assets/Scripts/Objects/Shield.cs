﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Objects
{
    public class Shield : MonoBehaviour
    {
        private Material shieldMaterial;
        private MeshRenderer meshRenderer;
        private bool shieldActive;
        [SerializeField] private float rippleDuration;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            shieldMaterial = meshRenderer.material;
        }

        public void StartRippleCoroutine(Vector3 pos)
        {
            StartCoroutine(RippleCoroutine(pos));
        }

        public IEnumerator RippleCoroutine(Vector3 worldStartPos )
        {
            float currentTime = 0f;
            Vector3 localStartPos = transform.InverseTransformPoint(worldStartPos);
            shieldMaterial.SetVector("_HitOrigin", localStartPos);
            

            // Increase amplitude from 0 to 0.1
            while (currentTime < rippleDuration)
            {
                float alpha = currentTime / rippleDuration;
                float amplitude = Mathf.Lerp(0f, 0.3f, alpha);
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
                float amplitude = Mathf.Lerp(0.3f, 0f, alpha);
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
                //StartCoroutine(RippleCoroutine());
            }
        }

        private void ToggleShield()
        {
            if (!shieldActive)
            {
                shieldActive = true;
                shieldMaterial.DOFloat(0.001f, "_DissolveAmount", 0.5f).SetEase(Ease.InOutSine);

            }
            else
            {
                shieldActive = false;
                shieldMaterial.DOFloat(1f,"_DissolveAmount",0.5f).SetEase(Ease.InQuad);
                
            }
           
        }
    }

   
}