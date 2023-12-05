
using UnityEngine;
using User;

namespace Objects
{
    public class TurretArea : MonoBehaviour
    {
        [SerializeField] private Turret turret;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerBody>(out var playerBody))
            {
                turret.isShooting = true;
            }
           
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerBody>(out var playerBody))
            {
                turret.isShooting = false;
            }
            
        }
    }
}