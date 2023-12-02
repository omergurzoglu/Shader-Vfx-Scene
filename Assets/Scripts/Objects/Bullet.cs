using System;
using System.Collections;
using UnityEngine;


namespace Objects
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 60f;
        private TrailRenderer trailRenderer;
        public Vector3 moveDirection;
        public static event Action<Vector3> OnBulletHit;

        
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
                OnBulletHit?.Invoke(transform.position);
                shield.StartRippleCoroutine(transform.position);
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


    