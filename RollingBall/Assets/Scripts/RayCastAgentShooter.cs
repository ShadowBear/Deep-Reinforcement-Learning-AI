using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RayCastAgentShooter : Agent
{
    public Transform Target;
    private Rigidbody rigid;
    public float speed = 15f;
    public float maxSpeed = 15f;
    public float rotationSpeed = 10f;
    int fieldSize = 9;

    //Bullets
    public GameObject bullet;
    public Transform shotPos;
    private Rigidbody bullRigid;
    public float bulletSpeed = 500;

    public bool hit = false;
    private bool foundTarget = false;

    public LayerMask layerMask;
    public LayerMask floorMask;


    RayPerception rayPer;
    string[] detectableObjects;


    private bool shooting = false;
    private Transform FloorTrans;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        FloorTrans = transform.parent.GetChild(0).transform;
        rayPer = GetComponent<RayPerception>();
        detectableObjects = new string[] { "wall", "Goal", "block" };
    }

    public override void AgentReset()
    {
        RaycastHit rayHit;
        //Training with Raycast
        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, 3, floorMask))
        {
            //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.yellow);
            this.rigid.angularVelocity = Vector3.zero;
            this.rigid.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0) + FloorTrans.position;
            //Debug.Log("Außerhalb");
        }

        //Floor Position needed
        //Target.localPosition = new Vector3(Random.Range(-(fieldSize - 2), (fieldSize - 1)), 0.5f, Random.Range(-(fieldSize - 2), (fieldSize - 1)));
        int tmpX = Random.Range(-3, 3);
        if (tmpX > 0) tmpX -= 4;
        else if (tmpX > 0) tmpX += 4;
        else tmpX = Random.Range(0, 2) == 1 ? 4 : -4;

        int tmpY = Random.Range(-3, 3);
        if (tmpY > 0) tmpY -= 4;
        else if (tmpY > 0) tmpY += 4;
        else tmpY = Random.Range(0, 2) == 1 ? 4 : -4;

        Target.localPosition = new Vector3(tmpX,0.5f, tmpY);
        hit = false;
        foundTarget = false;
    }


    public override void CollectObservations()
    {
        float distanceToTarget = (Vector3.Distance(this.transform.localPosition, Target.localPosition)) / (fieldSize);

        Vector3 targetDir = Target.localPosition - transform.localPosition;
        float angle = ((int)Vector3.Angle(targetDir, transform.forward)) / 180;

        float rayDistance = 20f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };

        AddVectorObs(rayPer.Perceive(
            rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(
            rayDistance, rayAngles, detectableObjects, 2.5f, 2.5f));

        AddVectorObs(this.transform.localPosition / fieldSize);
        AddVectorObs(Target.transform.localPosition / fieldSize);

        AddVectorObs(distanceToTarget);
        AddVectorObs(angle);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Actions
        int moveForward = (int) vectorAction[0];
        //Vector3 movement = transform.forward * moveForward * speed * Time.deltaTime;
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

        int rotation = (int) vectorAction[1];
        if(rotation == 1) transform.Rotate(Vector3.up * rotationSpeed, Space.World);
        else if(rotation == 2) transform.Rotate(Vector3.up * -rotationSpeed, Space.World);

        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        //if(distanceToTarget < 2)
        //{
        //    SetReward(1f);
        //    if (Time.timeScale == 1) Debug.Log("gefunden");
        //    Done();
        //}


        int shoot = (int) vectorAction[2];

        #region Shooting Commented
        if (shoot == 1)
        {
            if (distanceToTarget < 7)
            {
                //if (!foundTarget)
                //{
                //    AddReward(0.2f);
                //    foundTarget = true;
                //}
                RaycastHit hit;
                //Training with Raycast
                if (Physics.Raycast(shotPos.position, shotPos.TransformDirection(Vector3.forward), out hit, 10, layerMask))
                {
                    if (hit.collider == Target.GetComponent<BoxCollider>())
                    {
                        if (Time.timeScale == 1) Debug.Log("Did Hit");
                        SetReward(1f);
                        Done();
                        //StartCoroutine(ShootIt());
                    }
                    else
                    {
                        if (Time.timeScale == 1) Debug.Log("Did not Hit");
                        AddReward(-0.0005f);
                        //Done();
                    }
                }
                else
                {
                    //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.white);
                    if (Time.timeScale == 1) Debug.Log("Did not Hit");
                    AddReward(-0.005f);
                    //Done();
                }
            }
            //else
            //{
            //    //SetReward(-0.1f);
            //    //Done();
            //}
            //StartCoroutine(ShootIt());
        }

        //Rewards
        //AddReward(-0.05f);
        //if (distanceToTarget < 4f)
        //{
        //    //SetReward(-0.1f);
        //    Done();
        //    if (Time.timeScale == 1) Debug.Log("Verloren");
        //}
        #endregion

        if (hit)
        {
            if (Time.timeScale == 1) Debug.Log("Win");
            Done();
        }

        if (transform.position.y < -0.1f)
        {
            Done();
        }

        //Reset if Agent Fell off
        //RaycastHit rayHit;
        ////Training with Raycast
        //if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, 3, floorMask))
        //{
        //    Done();
        //    //Debug.Log("Außerhalb");
        //}


    }


    /* Shooting Training Raycast Stehend ************************************

    public override void CollectObservations()
    {
        Vector3 targetDir = Target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        AddVectorObs(angle / 180);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float rotation = vectorAction[0];
        transform.Rotate(Vector3.up * rotation * rotationSpeed, Space.World);

        float shoot = vectorAction[1];
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        if (shoot == 1)
        {
            RaycastHit hit;
            //Training with Raycast
            if (Physics.Raycast(shotPos.position, shotPos.TransformDirection(Vector3.forward), out hit, 10, layerMask))
            {
                //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.yellow);
                if (Time.timeScale == 1) Debug.Log("Did Hit");
                SetReward(1f);
                Done();
            }
            else
            {
                //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.white);
                if (Time.timeScale == 1) Debug.Log("Did not Hit");
                Done();
            }
            //StartCoroutine(ShootIt());
        }

        if (distanceToTarget < 1.42f)Done();
        if (hit)Done();
    }

    ******************************************************************* */

    IEnumerator ShootIt()
    {
        if (!shooting)
        {
            shooting = true;
            GameObject bull = Instantiate(bullet, shotPos.position, Quaternion.identity);
            bull.transform.forward = shotPos.forward;
            bull.GetComponent<Bullet>().SetDependingAgent(gameObject);
            bullRigid = bull.GetComponent<Rigidbody>();
            bullRigid.AddForce(bull.transform.forward * bulletSpeed);
            yield return new WaitForSeconds(0.5f);
            shooting = false;
        }
        yield return null;
        
    }
}
