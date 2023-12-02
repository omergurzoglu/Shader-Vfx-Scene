using System;
using Managers;
using UnityEngine;
using User;
using Random = UnityEngine.Random;

namespace Objects
{
    public class Turret : MonoBehaviour
    {
        public Transform player;
        public float fireRate = 0.5f;
        private float nextFireTime = 0f;
        [SerializeField] private bool isShooting;
        [SerializeField] private Transform bulletSpawnPos;
        private void Update()
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer = transform.InverseTransformDirection(directionToPlayer); // Convert to local space
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            angle += 90;
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, angle);
            if (Time.time >= nextFireTime && isShooting)
            {
                Fire();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        private void Fire()
        {
            Bullet bullet = BulletPool.GetBullet();
            if (bullet != null)
            {
                bullet.transform.position = bulletSpawnPos.position;
                Vector3 directionToPlayer = (player.position - bulletSpawnPos.position).normalized;
                
                float recoilMagnitude = 0.06f; // Adjust this value for more or less recoil
                Vector3 recoilOffset = new Vector3(
                    Random.Range(-recoilMagnitude, recoilMagnitude), // x-axis offset
                    Random.Range(-recoilMagnitude, recoilMagnitude), // y-axis offset
                    0 // z-axis offset, assuming you're working in 2D
                );
                Vector3 directionWithRecoil = directionToPlayer + recoilOffset;
                directionWithRecoil.Normalize(); // Re-normalize the vector after applying the offset
                
                bullet.Activate(directionWithRecoil);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerBody>(out var playerBody))
            {
                isShooting = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerBody>(out var playerBody))
            {
                isShooting = false;
            }
        }
    }
}