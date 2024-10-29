using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;

public class EnemyController : MonoBehaviour
{
    public bool hasPowerUp = false;
    private bool isStartUpdating = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2.0f);
        isStartUpdating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStartUpdating)
        {
            CheckPowerUp();
        }
    }

    private void CheckPowerUp()
    {
        hasPowerUp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().hasPowerUp;

        if (hasPowerUp)
        {
            this.GetComponent<GettingAway>().enabled = true;
            this.GetComponent<Approaching>().enabled = false;
        }
        else
        {
            this.GetComponent<GettingAway>().enabled = false;
            this.GetComponent<Approaching>().enabled = true;
        }
    }
}
