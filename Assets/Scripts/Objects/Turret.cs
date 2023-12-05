using Disc;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    public class Turret : MonoBehaviour
    {
        public Transform player;
        public float fireRate = 0.5f;
        private float nextFireTime = 0f;
        [SerializeField] private Transform mainPivot;
        [SerializeField] public bool isShooting;
        [SerializeField] private Transform bulletSpawnPos;
        [SerializeField] private ExplosionEffect turretExplosionVfx;
        
        [SerializeField] private float radius = 25f;
        [SerializeField] private float movementSpeed = 2f;
        private float angle = 0f;
        [SerializeField] private float yAmplitude = 2f;
        [SerializeField] private float ySpeed = 0.5f;
        
        private void Update()
        {
           // MoveInCircle();
            Shoot();
        }
        private void MoveInCircle()
        {
            angle += movementSpeed * Time.deltaTime;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            //float y = Mathf.Sin(angle * ySpeed) * yAmplitude;


            // Assuming you want to move in the XZ plane around the mainPivot
            mainPivot.position += mainPivot.position + new Vector3(x, 0, z);
        }

        private void Shoot()
        {
            if (isShooting)
            {
                Vector3 directionToPlayer = player.position - transform.position;
                directionToPlayer = transform.InverseTransformDirection(directionToPlayer); 
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
        }

        private void Fire()
        {
            Bullet bullet = BulletPool.GetBullet();
            if (bullet != null)
            {
                bullet.transform.position = bulletSpawnPos.position;
                Vector3 directionToPlayer = (player.position - bulletSpawnPos.position).normalized;
                
                float recoilMagnitude = 0.06f; 
                Vector3 recoilOffset = new Vector3(
                    Random.Range(-recoilMagnitude, recoilMagnitude), 
                    Random.Range(-recoilMagnitude, recoilMagnitude), 
                    0 
                );
                Vector3 directionWithRecoil = directionToPlayer + recoilOffset;
                directionWithRecoil.Normalize(); 
                
                bullet.Activate(directionWithRecoil);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<DiscObject>(out var disc))
            {
                TurretExplosion();
            }
        }

        private void TurretExplosion()
        {
            turretExplosionVfx.PlayEffect(transform.position);
        }

    
    }
}