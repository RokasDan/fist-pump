using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class GroundedController : MonoBehaviour
    {
        private int isInAir;

        private RaycastHit rayHit;


        // Ground check class, use the raycast lenght for the hover component to cast this ray.
        public bool IsGrounded(float hoverRayLenght)
        {
            return Physics.CheckSphere(transform.position - new Vector3(0, hoverRayLenght, 0), 0.5f) ||
                   Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), hoverRayLenght);
        }
    }
}
