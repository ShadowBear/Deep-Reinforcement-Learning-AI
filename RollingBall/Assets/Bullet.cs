using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject agent;
    void Start()
    {
        Destroy(gameObject, .75f);
    }

    public void SetDependingAgent(GameObject agent)
    {
        this.agent = agent;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            if(agent)agent.GetComponent<ShooterAgent>().hit = true;
            Destroy(gameObject);
        }
    }
}
