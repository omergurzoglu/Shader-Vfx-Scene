
using System.Collections;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.VFX;
namespace Objects
{
    public class Dummy : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float dissolveDuration=0.5f;

        private Material dummyMaterial;

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
            StartCoroutine(DissolveOverTime(dissolveDuration)); // 2 seconds for dissolve effect
        }

        private IEnumerator DissolveOverTime(float duration)
        {
            float currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0, 1, currentTime / duration);
                dummyMaterial.SetFloat("_DissolveAmount", dissolveAmount);
                yield return null;
            }
            dummyMaterial.SetFloat("_DissolveAmount", 1); 
        }

       
        
    }
}