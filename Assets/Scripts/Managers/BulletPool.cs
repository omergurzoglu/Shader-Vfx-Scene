using System;
using System.Collections.Generic;
using Objects;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class BulletPool : MonoBehaviour
    {
        public Bullet bulletPrefab;
        public int poolSize = 10;
        private List<Bullet> bulletPool;
        public static event Func<Bullet> RequestBullet;


        private void OnEnable()
        {
            RequestBullet += GetPooledBullet;
        }
        private void OnDisable()
        {
            RequestBullet -= GetPooledBullet;
        }
        public static Bullet GetBullet()
        {
            return RequestBullet?.Invoke();
        }

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