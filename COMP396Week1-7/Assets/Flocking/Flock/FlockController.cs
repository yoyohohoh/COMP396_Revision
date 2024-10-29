using UnityEngine;
using System. Collections;
using System. Collections.Generic;
public class FlockController : MonoBehaviour {
public float minVelocity = 1;
public float maxVelocity = 8;
public int flockSize = 20;
public float centerWeight = 1;
public float velocityWeight = 1;
public float separationWeight = 1;
public float followWeight = 1;
public float randomizeWeight = 1;
public Flock prefab;
public Transform target;
public Vector3 flockCenter;
internal Vector3 flockVelocity;

public ArrayList flockList = new ArrayList();
void Start () {
for (int i = 0; i < flockSize; i++) {
    Flock flock = Instantiate(prefab, transform.position, transform.rotation) as Flock;

flock.transform.parent = transform;
flock.controller = this;
flockList.Add(flock);

}
}
void Update() {
//Calculate the Center and Velocity of the whole flock group
Vector3 center = Vector3.zero;
Vector3 velocity = Vector3.zero;
foreach (Flock flock in flockList) {
center += flock.transform.localPosition;
velocity += flock.GetComponent<Rigidbody>().velocity;
}
flockCenter = center / flockSize;
flockVelocity = velocity / flockSize;
}

}