using System.Collections;
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
        [SerializeField] private Transform explosionStartPos;
        [SerializeField] public bool isShooting;
        [SerializeField] private Transform bulletSpawnPos;
        [SerializeField] private ExplosionEffect turretExplosionVfx;
        private Material[] materials = new Material[2];
        [SerializeField] private MeshRenderer[] meshRenderers;
        [SerializeField] private Color[] colors = new Color[10]; // Add your 10 colors here
        private float maxRippleTime = 4.5f;
        private static readonly int RippleTime = Shader.PropertyToID("_RippleTime");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        private void Awake()
        {
            for (var i = 0; i < meshRenderers.Length; i++)
            {
                var meshRenderer = meshRenderers[i];
                materials[i] = meshRenderer.material;
            }
        }

        private void Start()
        {
            StartCoroutine(ChangeColorAndRipple());
        }

        private IEnumerator ChangeColorAndRipple()
        {
            int colorIndex = 0;
            float colorChangeInterval = 0.012f;
            float colorChangeTimer = 0f;

            float rippleTime = 0f;
            float rippleIncrement = 0.35f; 
            bool isRippleTimeIncreasing = true;
            float rippleChangeInterval = 0.06f; 
            float rippleChangeTimer = 0f;

            while (true)
            {
                colorChangeTimer += Time.deltaTime;
                if (colorChangeTimer >= colorChangeInterval)
                {
                    foreach (var material in materials)
                    {
                        material.SetColor(Color1, colors[colorIndex] * 40);
                    }
                    colorIndex = (colorIndex + 1) % colors.Length;
                    colorChangeTimer = 0f;
                }
                
                rippleChangeTimer += Time.deltaTime;
                if (rippleChangeTimer >= rippleChangeInterval)
                {
                    if (isRippleTimeIncreasing)
                    {
                        rippleTime += rippleIncrement;
                        if (rippleTime >= maxRippleTime)
                        {
                            rippleTime = maxRippleTime;
                            isRippleTimeIncreasing = false;
                        }
                    }
                    else
                    {
                        rippleTime -= rippleIncrement;
                        if (rippleTime <= 0)
                        {
                            rippleTime = 0;
                            isRippleTimeIncreasing = true;
                        }
                    }
                    foreach (var material in materials)
                    {
                        material.SetFloat(RippleTime, rippleTime);
                    }
                    rippleChangeTimer = 0f;
                }
                yield return null; 
            }
        }

        private void Update()
        {
            Shoot();
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
            if (other.TryGetComponent<DiscObject>(out _))
            {
                //TurretExplosion();
            }
        }

        public void TurretExplosion()
        {
           
            turretExplosionVfx.PlayEffect(explosionStartPos.position);
        }

    
    }
}