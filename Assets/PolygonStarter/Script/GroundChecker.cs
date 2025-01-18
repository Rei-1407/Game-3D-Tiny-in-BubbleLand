using UnityEngine;

namespace Platformer {
    public class GroundChecker : MonoBehaviour {
        [SerializeField] float groundDistance = 0.08f;
        [SerializeField] LayerMask groundLayers;
        
        public bool IsGrounded { get; private set; }

        void Update() {
            IsGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}