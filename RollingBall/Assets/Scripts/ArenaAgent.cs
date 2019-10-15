using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MLAgents;

public class ArenaAgent : Agent
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

    public bool died = false;
    [SerializeField] Transform[] startingPoints;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        FloorTrans = transform.parent.GetChild(0).transform;
        rayPer = GetComponent<RayPerception>();
        detectableObjects = new string[] { "wall", "Agent", "block" };
    }

    public override void AgentReset()
    {
        if (died)
        {
            died = false;
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
            transform.position = startingPoints[Random.Range(0, startingPoints.Length)].position;
        }
        Target.position = new Vector3(Random.Range(-(fieldSize - 2), (fieldSize - 2)) + FloorTrans.position.x, 0.5f, Random.Range(-(fieldSize - 2), (fieldSize - 2)) + FloorTrans.position.z);

        //rigid.angularVelocity = Vector3.zero;
        //rigid.velocity = Vector3.zero;
        //transform.position = startingPoints[Random.Range(0, startingPoints.Length)].position;

    }


    public override void CollectObservations()
    {
        //float distanceToTarget = (Vector3.Distance(this.transform.localPosition, Target.localPosition)) / (fieldSize);
        float distance = (transform.localPosition.normalized - Target.localPosition.normalized).magnitude;

        Vector3 targetDir = Target.localPosition - transform.localPosition;
        float angle = ((int)Vector3.Angle(targetDir, transform.forward)) / 180;

        float rayDistance = 20f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };

        AddVectorObs(rayPer.Perceive(
            rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(
            rayDistance, rayAngles, detectableObjects, 2.5f, 2.5f));

        AddVectorObs(this.transform.localPosition / (fieldSize - 2));
        AddVectorObs(Target.transform.localPosition / (fieldSize - 2));
        
        //AddVectorObs(distanceToTarget);
        AddVectorObs(distance);
        AddVectorObs(angle);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.0005f);
        if (died)
        {
            //SetReward(-1);
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

        #region Shooting Commented
        if (shoot == 1)
        {
            if (distanceToTarget < 7)
            {
                RaycastHit hit;
                //Training with Raycast
                if (Physics.Raycast(shotPos.position, shotPos.TransformDirection(Vector3.forward), out hit, 10, layerMask))
                {
                    if (hit.collider == Target.GetComponent<BoxCollider>())
                    {
                        if (Time.timeScale == 1) Debug.Log("Did Hit");
                        if(Target.GetComponent<ArenaAgent>()) Target.GetComponent<ArenaAgent>().died = true;
                        SetReward(1f);
                        Done();
                    }
                    else
                    {
                        if (Time.timeScale == 1) Debug.Log("Did not Hit");
                        //AddReward(-0.005f);
                    }
                }
                else
                {
                    //Debug.DrawRay(shotPos.position, shotPos.TransformDirection(Vector3.forward) * 10, Color.white);
                    if (Time.timeScale == 1) Debug.Log("Did not Hit");
                    //AddReward(-0.005f);
                }
            }
            //else AddReward(-0.0005f);
        }

        if (transform.position.y > 1 || transform.position.y < 0)
        {
            //Debug.Log("Rausgeworfen");
            died = true;
        }

        #endregion
    }

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
