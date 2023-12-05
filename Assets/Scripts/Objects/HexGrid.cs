
using System.Collections;

using UnityEngine;

namespace Objects
{
    public class HexGrid : MonoBehaviour
    {
        public Material material;
        private MeshRenderer meshRenderer;
        private static readonly int HitPosition = Shader.PropertyToID("_HitPosition");
        private static readonly int RippleTime = Shader.PropertyToID("_RippleTime");
        public bool effectActive;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.material;
        }

        public IEnumerator HexScan(Vector3 startPos)
        {
            effectActive = true;
            material.SetVector(HitPosition,startPos);
            float currentTime = 0f; 
            while (currentTime < 5f)
            {
                currentTime += Time.deltaTime;
                float lerpValue = Mathf.Lerp(0f, 90f, currentTime / 5f);
                material.SetFloat(RippleTime, lerpValue);
                yield return null; 
            }
            material.SetFloat(RippleTime, -2);
            effectActive = false;
        }
    }

 
}