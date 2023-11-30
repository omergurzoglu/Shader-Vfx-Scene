using System.Collections;
using UnityEngine;
using User;

namespace Objects
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 75f;
        private TrailRenderer trailRenderer;
        public Vector3 moveDirection;
        
        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        
        private void Update()
        {
                transform.position += moveDirection * (speed * Time.deltaTime);
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
        
        public void Activate(Vector3 dir)
        {
            moveDirection = dir;
            gameObject.SetActive(true);
            StartCoroutine(DeactivationCoroutine());
        }
        private IEnumerator DeactivationCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Deactivate();
        }

        private void Deactivate()
        {
            StopAllCoroutines(); 
            trailRenderer.Clear();
            gameObject.SetActive(false);
        }
    }
}


    