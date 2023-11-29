using System.Collections.Generic;
using UnityEngine;
namespace Objects
{
    public class Turret : MonoBehaviour
    {
        public Transform player;
        public Bullet bulletPrefab;
        public int poolSize = 10;
        private List<Bullet> bulletPool;
        public float fireRate = 0.5f;
        private float nextFireTime = 0f;
        [SerializeField] private Transform bulletSpawnPos;

        private void Start()
        {
            bulletPool = new List<Bullet>();
            for (int i = 0; i < poolSize; i++)
            {
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.gameObject.SetActive(false);
                bulletPool.Add(bullet);
            }
        }
        private void Update()
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer = transform.InverseTransformDirection(directionToPlayer); // Convert to local space
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            angle += 90;
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, angle);
            
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        private void Fire()
        {
            var bullet = GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position =bulletSpawnPos.position;
                bullet.gameObject.SetActive(true);
                bullet.SetTarget(player);
            }
        }
        private Bullet GetPooledBullet()
        {
            foreach (var bullet in bulletPool)
            {
                if (!bullet.gameObject.activeInHierarchy)
                {
                    return bullet;
                }
            }
            return null; 
        }
    }
}