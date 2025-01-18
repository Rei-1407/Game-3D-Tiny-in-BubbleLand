using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
   Rigidbody rb;
    public Vector3 boxSize;
    public float maxDistance;   
    public LayerMask layerMask;
    public bool grounded;
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
       
        if(IsGrounded())
        {
            rb.velocity=Vector3.zero;
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
    public bool IsGrounded()
    {
        return  Physics.CheckSphere(transform.position, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
;
    }
}
