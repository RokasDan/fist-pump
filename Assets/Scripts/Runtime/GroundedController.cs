using UnityEngine;
using UnityEngine.Events;


namespace RokasDan.FistPump.Runtime
{
    public class GroundedController : MonoBehaviour
    {
        // Event, here users can specify functions they want to use when these events happen:
        [SerializeField]
        private UnityEvent onEnteredGrounded;

        [SerializeField]
        private UnityEvent onExitedGrounded;

        [Min(1f)]
        [SerializeField]
        private float detectionHeight;
        [SerializeField]
        private float radius;

        private Vector3 p1;
        private Vector3 rayDistance;

        // Method to check if the object is grounded.
        public bool IsGrounded { get; set; }
        public float GroundDistance { get; set; }

        // Need to speak about this, need to pass this to other class methods??????????????
        public RaycastHit RayHit;

        // Ground check with events and bool value for direct use.
        public void UpdateGrounded()
        {
            // Creating the position for the first sphere. We this we make so that the distance
            // of the raycast will be smallest on the middle of the object.
            p1 = transform.position + new Vector3(0, radius, 0);

            if (Physics.SphereCast(p1, radius, Vector3.down, out RayHit, detectionHeight))
            {
                if (IsGrounded == false)
                {
                    onEnteredGrounded?.Invoke();
                }

                IsGrounded = true;
            }
            else
            {
                if (IsGrounded)
                {
                    onExitedGrounded?.Invoke();
                }

                IsGrounded = false;
            }

            GroundDistance = RayHit.distance;
            rayDistance = new Vector3(0, GroundDistance, 0);
        }

        private void OnDrawGizmos()
        {
            //Drawing the sphere cast collider.
            var position = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position + new Vector3(-radius, radius, 0), position - new Vector3(radius, detectionHeight - radius, 0));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position + new Vector3(radius, radius, 0), position - new Vector3(-radius, detectionHeight - radius, 0));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position + new Vector3(0, radius, -radius), position - new Vector3(0, detectionHeight - radius, radius));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position + new Vector3(0, radius, radius), position - new Vector3(0, detectionHeight - radius, -radius));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position + new Vector3(0, radius, 0), radius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(position - new Vector3(0, detectionHeight - radius, 0), radius);

            // How deep the collider is currently hitting, maybe should be at the hover controller?
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(position - rayDistance, radius);
        }
    }
}
