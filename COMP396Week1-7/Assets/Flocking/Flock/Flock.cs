
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    internal FlockController controller;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update () {
        if (controller) {
        Vector3 relativePos = Steer() * Time.deltaTime;
        if (relativePos != Vector3.zero)
        rb.velocity = relativePos;
        // enforce minimum and maximum speeds for the boids
        float speed = rb.velocity.magnitude;
        if (speed > controller.maxVelocity) {
        rb.velocity = rb.velocity.normalized * controller.maxVelocity;
        } else if (speed < controller.minVelocity) {
        rb.velocity = rb.velocity.normalized * controller.minVelocity;
        }
    }
    }

    private Vector3 Steer () {

    Vector3 center = controller.flockCenter - transform.localPosition;

    //cohesion
    Vector3 velocity = controller. flockVelocity - rb.velocity;
    //allignement
    Vector3 follow = controller.target.localPosition - transform.localPosition; 
    //follow leader
    Vector3 separation = Vector3.zero;
    foreach (Flock flock in controller.flockList) {
    if (flock != this) {
    Vector3 relativePos = transform. localPosition - flock.transform.localPosition;
    separation += relativePos.normalized;
    }
    }

    // randomize
    Vector3 randomize = new Vector3( (Random.value * 2) - 1, (Random.value * 2) - 1,
    (Random.value * 2) - 1);
    randomize. Normalize();
    return (controller.centerWeight * center +
    controller.velocityWeight * velocity +
    controller.separationWeight * separation +
    controller.followWeight * follow +
    controller.randomizeWeight * randomize);

 }
}
