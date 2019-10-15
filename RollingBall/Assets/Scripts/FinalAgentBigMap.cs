using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class FinalAgentBigMap : Agent
{
    public Transform Target;
    private Rigidbody rigid;
    public float speed = 15f;
    public float maxSpeed = 15f;
    public float rotationSpeed = 10f;
    //int fieldSize = 90;    
    int fieldSize = 39;

    public bool hit = false;
    private bool foundTarget = false;

    public LayerMask layerMask;
    public LayerMask floorMask;


    RayPerception rayPer;
    string[] detectableObjects;


    private bool shooting = false;
    private Transform FloorTrans;

    public bool died = false;
    public bool enemyKilled = false;
    [SerializeField] Transform[] startingPoints;

    TankHealth enemyHealthScript;
    //TankHealthHardcoded enemyHealthScript;
    TankHealth healthScript;
    TankShooting tankShooting;
    public float forceMultiplier = 1;

    public int score = 0;
    private bool scored = false;
    [SerializeField] int ids;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        FloorTrans = transform.parent.GetChild(0).transform;
        rayPer = GetComponent<RayPerception>();
        detectableObjects = new string[] { "wall", "Agent", "block" };

        //Für FollowAgent Auskommentieren
        enemyHealthScript = Target.GetComponent<TankHealth>();

        //enemyHealthScript = Target.GetComponent<TankHealthHardcoded>();


        healthScript = GetComponent<TankHealth>();
        tankShooting = GetComponent<TankShooting>();
    }

    public override void AgentReset()
    {
        died = false;
        enemyKilled = false;
        rigid.angularVelocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
        transform.position = startingPoints[Random.Range(0, startingPoints.Length)].position;
        healthScript.Start();

        //scored = false;
    }


    public override void CollectObservations()
    {
        //fieldsize^2 + fieldsize^2 = maxDistance^2 --> Teilen
        //float distanceToTarget = (Vector3.Distance(this.transform.localPosition, Target.localPosition) / 127.28f);
        float distanceToTarget = (Vector3.Distance(this.transform.localPosition, Target.localPosition) / 55.2f);

        Vector3 targetDir = Target.localPosition - transform.localPosition;
        int angle = ((int)Vector3.Angle(targetDir, transform.forward)) / 180;

        float rayDistance = 15f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };

        AddVectorObs(rayPer.Perceive(
            rayDistance, rayAngles, detectableObjects, 0f, 0f));

        AddVectorObs(this.transform.localPosition / (fieldSize/2));
        AddVectorObs(Target.transform.localPosition / (fieldSize/2));

        //Für FollowAgent Auskommentieren
        AddVectorObs(enemyHealthScript.m_CurrentHealth / 100f);

        AddVectorObs(healthScript.m_CurrentHealth / 100f);
        AddVectorObs(tankShooting.canFire);

        AddVectorObs(distanceToTarget);
        AddVectorObs(angle);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.0001f);
        if (died)
        {
            //Only used when normal speed
            if (Time.timeScale <= 5 && !scored)
            {
                scored = true;
                ScoreIt(0);
            }

            Done();
        }else if (enemyKilled)
        {
            //Only used when normal speed
            if (Time.timeScale <= 4 && !scored)
            {
                scored = true;
                ScoreIt(1);
            }

            //float reward = (healthScript.m_CurrentHealth / 100f) * 0.5f;
            //SetReward(0.5f + reward);
            SetReward(1f);
            Done();
        }

        //Actions
        int moveForward = (int)vectorAction[0];
        if (moveForward == 1)
        {
            Vector3 movement = transform.forward * speed * Time.deltaTime;
            rigid.MovePosition(rigid.position + movement);
        }
        else if (moveForward == 2)
        {
            Vector3 movement = transform.forward * -speed * Time.deltaTime;
            rigid.MovePosition(rigid.position + movement);
        }

        int rotation = (int)vectorAction[1];
        if (rotation == 1) transform.Rotate(Vector3.up * rotationSpeed, Space.World);
        else if (rotation == 2) transform.Rotate(Vector3.up * -rotationSpeed, Space.World);

        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        int shoot = (int)vectorAction[2];


        if (shoot == 1 && tankShooting.canFire)
        {
            float launchForce = distanceToTarget * forceMultiplier;
            tankShooting.Fire(launchForce);
        }

        if (transform.position.y > 1 || transform.position.y < 0)
        {
            died = true;
        }

    }

    public void DamageReward(float r)
    {
        AddReward(r);
    }

    private void ScoreIt(int won)
    {
        score += won;
        Debug.Log("Agent " + ids + " = " + score);
        StartCoroutine(Scored());
    }   


    IEnumerator Scored()
    {
        yield return new WaitForSeconds(1.5f);
        scored = false;
        yield return null;
    }

}
