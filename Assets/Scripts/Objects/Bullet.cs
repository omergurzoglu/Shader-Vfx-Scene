using UnityEngine;
using User;

namespace Objects
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 75f;
        public Transform target;
        private TrailRenderer trailRenderer;
        
        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        private void Update()
        {
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * (speed * Time.deltaTime);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Shield>(out var shield))
            {
                Deactivate();
            }

            if (other.TryGetComponent<PlayerBody>(out var player))
            {
                Deactivate();
            }
        }
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void Deactivate()
        {
            trailRenderer.Clear();
            gameObject.SetActive(false);
        }
    }
}


    