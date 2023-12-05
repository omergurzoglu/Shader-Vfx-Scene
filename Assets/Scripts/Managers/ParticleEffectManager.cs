using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.VFX;

namespace Managers
{
    public class ParticleEffectManager : MonoBehaviour
    {
        
        public int poolSize = 20; // Initial size of the pool
        [SerializeField] private VisualEffect bulletImpactVfx;
        private Queue<VisualEffect> particlePool = new Queue<VisualEffect>();
        private void Awake()
        {
            InitializePool();
        }
        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var newParticle = Instantiate(bulletImpactVfx, transform);
                newParticle.gameObject.SetActive(false);
                particlePool.Enqueue(newParticle);
            }
        }
        private void PlayBulletImpact(Vector3 position)
        {
            
            if (particlePool.Count > 0)
            {
                var particle = particlePool.Dequeue();
                particle.transform.position = position;
                particle.gameObject.SetActive(true);
                particle.Play();

                StartCoroutine(DeactivateAndEnqueueParticle(particle));
            }
        }
        private IEnumerator DeactivateAndEnqueueParticle(VisualEffect particle)
        {
            yield return new WaitForSeconds(0.4f);
            particle.gameObject.SetActive(false);
            particlePool.Enqueue(particle);
        }
        
        private void OnEnable()
        {
            Bullet.OnBulletHit += PlayBulletImpact;
        }

        private void OnDisable()
        {
            Bullet.OnBulletHit -= PlayBulletImpact;
        }
        

    }
}