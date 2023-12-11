using System.Collections;
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
        private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
        private static readonly int HitOrigin = Shader.PropertyToID("_HitOrigin");

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
            shieldMaterial.SetVector(HitOrigin, localStartPos);
            
            while (currentTime < rippleDuration)
            {
                float alpha = currentTime / rippleDuration;
                float amplitude = Mathf.Lerp(0f, 0.3f, alpha);
                shieldMaterial.SetFloat(Amplitude, amplitude);
                currentTime += Time.deltaTime;
                yield return null;
            }
            currentTime = 0f;
            while (currentTime < rippleDuration)
            {
                float alpha = currentTime / rippleDuration;
                float amplitude = Mathf.Lerp(0.3f, 0f, alpha);
                shieldMaterial.SetFloat(Amplitude, amplitude);
                currentTime += Time.deltaTime;
                yield return null;
            }
            shieldMaterial.SetFloat(Amplitude,0f);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ToggleShield();
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