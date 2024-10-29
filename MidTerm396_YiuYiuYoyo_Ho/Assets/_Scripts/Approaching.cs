using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Approaching : MonoBehaviour
{
    #region Movement properties
    bool isMoving = true;
    private Vector3 tarPos;
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float rotSpeed = 2.0f;
    [SerializeField] private float minX = -9.0f;
    [SerializeField] private float maxX = 9.0f;
    [SerializeField] private float minZ = -9.0f;
    [SerializeField] private float maxZ = 9.0f;
    [SerializeField] private float targetReactionRadius = 5.0f;
    [SerializeField] private float targetVerticalOffset = 0.5f;
    #endregion

    #region Sense properties
    public GameObject goPlayer;
    public bool isVisible = false;
    public float detectionRate = 1.0f;
    private float elapsedTime = 0.0f;
    #endregion
    #region Sight properties
    public int FieldOfView = 90;
    public int ViewDistance = 8;
    private Transform playerTrans;
    private Vector3 rayDirection;
    #endregion
    void Start()
    {
        if (goPlayer == null)
        {
            goPlayer = GameObject.FindGameObjectWithTag("Player");
        }
        playerTrans = goPlayer.transform;
        GetNextPosition();
    }
    void Update()
    {
        if(isMoving)
        {
            if (isVisible)
            {

                MoveTowardsPlayer();
            }
            else
            {
                Movement();
            }
        }

        UpdateSense();
    }

    private void UpdateSense()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= detectionRate)
        {
            DetectAspect();
            elapsedTime = 0.0f;
        }
    }
    private void ChangeColor(Color newColor)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = newColor;
        }
    }
    private void DetectAspect()
    {
        if (playerTrans == null) return;

        rayDirection = (playerTrans.position - transform.position).normalized;

        if (Vector3.Angle(rayDirection, transform.forward) < FieldOfView)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                if (hit.collider.gameObject == goPlayer)
                {
                    Debug.Log("Player Detected");
                    isVisible = true;
                    ChangeColor(Color.red);
                }
            }
            else
            {
                Debug.Log("Player Undetected");
                isVisible = false;
                ChangeColor(Color.grey);
            }
        }
    }

    void GetNextPosition()
    {
        tarPos = new Vector3(Random.Range(minX, maxX), targetVerticalOffset, Random.Range(minZ, maxZ));
    }

    private void Movement()
    {
        if (Vector3.Distance(tarPos, transform.position) <= targetReactionRadius)
        {
            GetNextPosition();
        }

        Quaternion tarRot = Quaternion.LookRotation(tarPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, rotSpeed * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, movementSpeed * Time.deltaTime));
    }

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (playerTrans.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        transform.Translate(directionToPlayer * movementSpeed * Time.deltaTime, Space.World);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isEditor || playerTrans == null)
            return;

        Debug.DrawLine(transform.position, playerTrans.position, Color.red);
        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        Vector3 leftRayPoint = Quaternion.Euler(0, FieldOfView * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -FieldOfView * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == goPlayer)
        {
            Debug.Log("Caught Player");
            isMoving = false;
        }
    }
}


