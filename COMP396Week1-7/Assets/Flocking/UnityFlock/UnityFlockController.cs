using UnityEngine;
using System.Collections;

public class UnityFlockController : MonoBehaviour 
{
    public Vector3 bound;
    public float speed = 100.0f;
    public float targetReachedRadius = 10.0f;

    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;

    // Use this for initialization
    void Start () 
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }

    // Update is called once per frame
    void Update () 
    {        
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        Quaternion targetRot = Quaternion.LookRotation(nextMovementPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.5f * Time.deltaTime);
        if (Vector3.Distance(nextMovementPoint, transform.position) <= targetReachedRadius)
        {
            CalculateNextMovementPoint();
        }
    }    
    void CalculateNextMovementPoint () 
    {
        float posX = Random.Range(initialPosition.x - bound.x, initialPosition.x + bound.x);
        float posY = Random.Range(initialPosition.y - bound.y, initialPosition.y + bound.y);
        float posZ = Random.Range(initialPosition.z - bound.z, initialPosition.z + bound.z);

        nextMovementPoint = new Vector3(posX, posY, posZ);
    }
}
