using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgent : MonoBehaviour
{

    public FinalAgentBigMap enemyAgent;
    //public FollowAgent enemyAgent;
    private NavMeshAgent navMeshAgent;
    public bool alive;
    private float distanceToEnemy;
    private TankShooting tankShooting;
    private TankHealthHardcoded tankHealth;
    [SerializeField] Transform[] startingPoints;
    int score = 0;
    bool scored = false;
    public bool enemyKilled;
    public string name;


    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 4;
        navMeshAgent = GetComponent<NavMeshAgent>();
        tankShooting = GetComponent<TankShooting>();
        tankHealth = GetComponent<TankHealthHardcoded>();

        navMeshAgent.SetDestination(enemyAgent.transform.position);
        transform.position = startingPoints[Random.Range(0, startingPoints.Length)].position;
        tankHealth.Start();
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            navMeshAgent.SetDestination(enemyAgent.transform.position);
            if (navMeshAgent.remainingDistance > 3.5f) navMeshAgent.isStopped = false;
            else navMeshAgent.isStopped = true;

            distanceToEnemy = Mathf.Abs((transform.position - enemyAgent.transform.position).magnitude);
            if (distanceToEnemy < 10 && tankShooting.canFire)
            {
                tankShooting.Fire(distanceToEnemy);
            }
        }
        else
        {
            if(!scored) ResetAgent(0);
        }
        if (enemyAgent.died) if (!scored) ResetAgent(1);
        //if (enemyAgent.tankHealth.m_Dead) if (!scored) ResetAgent(1);

    }

    public void ResetAgent(int won)
    {
        scored = true;
        score += won;
        Debug.Log(name + " Score = " + score);
        if(won == 0) enemyAgent.enemyKilled = true;
        Start();
        StartCoroutine(Scored());
    }
    
    IEnumerator Scored()
    {
        yield return new WaitForSeconds(1.5f);
        scored = false;
        yield return null;
    }

    public void DamageReward(float dmg)
    {

    }
}
