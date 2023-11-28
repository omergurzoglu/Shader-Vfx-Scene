using UnityEngine;
using User;

namespace Objects
{
    public class JumpPadObject : MonoBehaviour
    {
        [SerializeField] private int jumpForce=200;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerMovement>(out var player))
            {
                player.JumpPad(jumpForce);
            }
        }
    }
}