using UnityEngine;
using System. Collections;
using static UnityEngine.UI.Image;
public class UnityFlock : MonoBehaviour
{
    public float minSpeed = 20.0f;
    public float turnSpeed = 20.0f;
    public float randomFreq = 20.0f;
    public float randomForce = 20.0f;
    //alignment variables
    public float toOriginForce = 50.0f;
    public float toOriginRange = 100.0f;
    public float gravity = 2.0f;
    //seperation variables
    public float avoidanceRadius = 50.0f;
    public float avoidanceForce = 20.0f;
    //cohesion variables
    public float followVelocity = 4.0f;
    public float followRadius = 40.0f;
    //these variables control the movement of the boid
    private Transform origin;
    private Vector3 velocity;
    private Vector3 normalizedVelocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    private Transform[] objects;
    private UnityFlock[] otherFlocks;
    private Transform transformComponent;
    private float randomFreqInterval;
    // Assign the parent as origin

    void Start()
    {
        randomFreqInterval = 1.0f / randomFreq;
        origin = transform.parent; // Keep the parent as the origin
        transformComponent = transform; // Flock transform
        Component[] tempFlocks = null; // Temporary components

        // Get all the unity flock components from the parent transform in the group
        if (origin)
        {
            tempFlocks = origin.GetComponentsInChildren<UnityFlock>();

            // Assign and store all the flock objects in this group
            objects = new Transform[tempFlocks.Length];
            otherFlocks = new UnityFlock[tempFlocks.Length];
            for (int i = 0; i < tempFlocks.Length; i++)
            {
                objects[i] = tempFlocks[i].transform;
                otherFlocks[i] = tempFlocks[i] as UnityFlock; // Use explicit casting here

                // Calculate random push depends on the random frequency provided
                StartCoroutine(UpdateRandom());
            }
        }
    }


    IEnumerator UpdateRandom()
        {
            while (true)
            {
                randomPush = Random.insideUnitSphere * randomForce;
                yield return new WaitForSeconds(randomFreqInterval + Random.Range(-randomFreqInterval / 2.0f, randomFreqInterval / 2.0f));
            }
        }

    void Update()
    {
        // Internal variables
        float speed = velocity.magnitude;
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        int count = 0;
        Vector3 myPosition = transformComponent.localPosition; // Use localPosition here
        Vector3 forceV;
        Vector3 toAvg;

        for (int i = 0; i < objects.Length; i++)
        {
            Transform boidTransform = objects[i];
            if (boidTransform != transformComponent)
            {
                Vector3 otherPosition = boidTransform.localPosition; // Use localPosition here
                                                                     // Average position to calculate cohesion
                avgPosition += otherPosition;
                count++;

                // Directional vector from other flock to this flock
                forceV = myPosition - otherPosition;

                // Magnitude of that directional vector (Length)
                float directionMagnitude = forceV.magnitude;
                float forceMagnitude = 0.0f;

                if (directionMagnitude < followRadius)
                {
                    if (directionMagnitude < avoidanceRadius)
                    {
                        forceMagnitude = 1.0f - (directionMagnitude / avoidanceRadius);
                        if (directionMagnitude > 0)
                            avgVelocity += (forceV / directionMagnitude) * forceMagnitude * avoidanceForce;
                    }

                    forceMagnitude = directionMagnitude / followRadius;
                    UnityFlock tempOtherBoid = otherFlocks[i];
                    avgVelocity += followVelocity * forceMagnitude * tempOtherBoid.normalizedVelocity;
                }
            }
        }

        if (count > 0)
        {
            // Calculate the average flock velocity (Alignment)
            avgVelocity /= count;

            // Calculate Center value of the flock (Cohesion)
            toAvg = (avgPosition / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }

        // Directional Vector to the leader
        forceV = origin.localPosition - myPosition; // Use localPosition here
        float leaderDirectionMagnitude = forceV.magnitude;
        float leaderForceMagnitude = leaderDirectionMagnitude / toOriginRange;

        // Calculate the velocity of the flock to the leader
        if (leaderDirectionMagnitude > 0)
            originPush = leaderForceMagnitude * toOriginForce * (forceV / leaderDirectionMagnitude);

        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }

        Vector3 wantedVel = velocity;

        // Calculate final velocity
        wantedVel -= wantedVel * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += originPush * Time.deltaTime;
        wantedVel += avgVelocity * Time.deltaTime;
        wantedVel += gravity * Time.deltaTime * toAvg.normalized;

        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.0f);
        transformComponent.rotation = Quaternion.LookRotation(velocity);

        // Move the flock based on the calculated velocity
        transformComponent.localPosition += velocity * Time.deltaTime; // Use localPosition here

        // Normalize the velocity for future reference
        normalizedVelocity = velocity.normalized;
    }


}

