
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
namespace Objects
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float dissolveDuration;

        private Material dummyMaterial;
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            vfx = GetComponentInChildren<VisualEffect>();
            dummyMaterial = meshRenderer.material;
        }

        public void DeathVfx(Vector3 hitDirection)
        {
            hitDirection.y = 0;
            hitDirection = -hitDirection*15f;
            vfx.SetVector3("GravityForce", hitDirection);
            vfx.Play();
        }
        public void Dissolve()
        {
            StartCoroutine(DissolveOverTime(dissolveDuration)); 
        }

        private IEnumerator DissolveOverTime(float duration)
        {
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0, 1, currentTime / duration);
                dummyMaterial.SetFloat(DissolveAmount, dissolveAmount);
                yield return null;
            }
            dummyMaterial.SetFloat(DissolveAmount, 1);
            yield return new WaitForSeconds(3.8f);
            gameObject.SetActive(false);
        }
        
    }
}