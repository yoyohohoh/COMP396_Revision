using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingAway : MonoBehaviour
{
    #region Movement properties
    bool isAvoiding = false;
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
    public bool hasPowerUp = false;
    public float detectionRate = 1.0f;
    private float elapsedTime = 0.0f;
    #endregion
    #region Sight properties
    public int FieldOfView = 360;
    public int ViewDistance = 5;
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
        hasPowerUp = goPlayer.GetComponent<PlayerController>().hasPowerUp;
        GetNextPosition();
    }
    void Update()
    {
        if(!isAvoiding)
        {
            Movement();
        }
        else
        {
            AvoidPlayer();
        }
        
        UpdatePowerUp();
        UpdateSense();
    }

    private void UpdatePowerUp()
    {
        hasPowerUp = goPlayer.GetComponent<PlayerController>().hasPowerUp;
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
                    if(hasPowerUp)
                    {
                        Debug.Log("Player has PowerUp and less than 5 away");
                        isAvoiding = true;
                        ChangeColor(Color.yellow);
                    }
                    else
                    {
                        Debug.Log("Player is close but has No PowerUp");
                        isAvoiding = false;
                        ChangeColor(Color.grey);
                    }

                }

            }
            else
            {
                Debug.Log("Player Undetected");
                isAvoiding = false;
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

    private void AvoidPlayer()
    {
        Vector3 awayFromPlayer = transform.position - playerTrans.position;
        awayFromPlayer.Normalize();

        Vector3 newPosition = transform.position + awayFromPlayer * movementSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        newPosition.y = targetVerticalOffset;

        Quaternion targetRotation = Quaternion.LookRotation(newPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, newPosition, movementSpeed * Time.deltaTime);
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
}
