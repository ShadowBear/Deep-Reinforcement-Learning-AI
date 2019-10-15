using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class ShooterAgent : Agent
{
    public Transform Target;
    private Rigidbody rigid;
    public float speed = 15f;
    public float maxSpeed = 15f;
    public float rotationSpeed = 10f;
    int fieldSize = 10;

    //Bullets
    public GameObject bullet;
    public Transform shotPos;
    private Rigidbody bullRigid;
    public float bulletSpeed = 500;

    public bool hit = false;
    private bool foundTarget = false;

    public LayerMask layerMask;
    public LayerMask floorMask;


    private bool shooting = false;
    private Transform FloorTrans;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        FloorTrans = transform.parent.GetChild(0).transform;
    }

    public override void AgentReset()
    {
        //if (this.transform.position.y < 0)
        //{
        //    this.rigid.angularVelocity = Vector3.zero;
        //    this.rigid.velocity = Vector3.zero;
        //    this.transform.position = new Vector3(0, 0.5f, 0);
        //}
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
        Target.position = new Vector3(Random.value * (fieldSize-2) - ((fieldSize/2)-1) + FloorTrans.position.x, 0.5f, Random.value * (fieldSize - 2) - ((fieldSize / 2) - 1) + FloorTrans.position.z);
        hit = false;
        foundTarget = false;
    }


    public override void CollectObservations()
    {
        //float targetPosX = ((int)Target.position.x / (fieldSize / 2));
        //float targetPosZ = ((int)Target.position.z / (fieldSize / 2));

        //float posX = ((int)transform.position.x / (fieldSize / 2));
        //float posZ = ((int)transform.position.z / (fieldSize / 2));

        //float distanceToTarget = ((int)Vector3.Distance(this.transform.position, Target.position))/ (fieldSize);
        float distanceToTarget = (Vector3.Distance(this.transform.position, Target.position)) / (fieldSize);

        Vector3 targetDir = Target.position - transform.position;
        float angle = ((int)Vector3.Angle(targetDir, transform.forward)) / 180;

        //AddVectorObs(posX);
        //AddVectorObs(posZ);
        //AddVectorObs(targetPosX);
        //AddVectorObs(targetPosZ);

        AddVectorObs(distanceToTarget);
        ////AddVectorObs(rigid.velocity.z/ maxSpeed);
        //AddVectorObs(rigid.velocity.z);
        //AddVectorObs(rigid.velocity.x);

        AddVectorObs(angle);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Actions
        float moveForward = vectorAction[0];
        Vector3 movement = transform.forward * moveForward * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + movement);

        float rotation = vectorAction[1];
        transform.Rotate(Vector3.up * rotation * rotationSpeed, Space.World);

        //float shoot = vectorAction[2];
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        if(distanceToTarget < 2)
        {
            SetReward(1f);
            if (Time.timeScale == 1) Debug.Log("gefunden");
            Done();
        }
        //if (shoot == 1)
        //{
        //    if (distanceToTarget < 5)
        //    {
        //        if (!foundTarget)
        //        {
        //            AddReward(0.2f);
        //            foundTarget = true;
        //        }
        //        RaycastHit hit;
        //        //Training with Raycast
        //        if (Physics.Raycast(shotPos.position, shotPos.TransformDirection(Vector3.forward), out hit, 10, layerMask))
        //        {
        //            if(hit.collider == Target.GetComponent<BoxCollider>())
        //            {
        //                if (Time.timeScale == 1) Debug.Log("Did Hit");
        //                SetReward(1f);
        //                Done();
        //                //StartCoroutine(ShootIt());
        //            }
        //            else
        //            {
        //                if (Time.timeScale == 1) Debug.Log("Did not Hit");
        //                SetReward(0.5f);
        //                Done();
        //            }
        //        }
        //        else
        //        {
        //            //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.white);
        //            if (Time.timeScale == 1) Debug.Log("Did not Hit");
        //            SetReward(0.5f);
        //            Done();
        //        }
        //    }
        //    else
        //    {
        //        //SetReward(-0.1f);
        //        Done();
        //    }
        //    //StartCoroutine(ShootIt());
        //}

        //Rewards
        //AddReward(-0.05f);
        //if (distanceToTarget < 4f)
        //{
        //    //SetReward(-0.1f);
        //    Done();
        //    if (Time.timeScale == 1) Debug.Log("Verloren");
        //}

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
