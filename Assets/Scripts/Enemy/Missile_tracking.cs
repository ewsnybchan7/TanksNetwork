using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Analytics;

public class Missile_tracking : MonoBehaviour
{
   
    private Transform missileTarget;
    public Rigidbody missileRigidbody;

    public float turn;
    public float missilseVelocity;
    private float elasped = 0;

    private void FixedUpdate()
    {
        elasped += Time.deltaTime;
       
            
        GameObject target = GameObject.FindGameObjectWithTag("usr");
        missileTarget = target.GetComponent<Transform>();
        if (elasped > 0.5f)
            missileRigidbody.velocity = transform.up * missilseVelocity;
        else
        {
            missileRigidbody.velocity = transform.forward * missilseVelocity;

        }

        var missileTargetRotation = Quaternion.LookRotation(missileTarget.position - transform.position);

        missileRigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, missileTargetRotation, turn));
    }
}
