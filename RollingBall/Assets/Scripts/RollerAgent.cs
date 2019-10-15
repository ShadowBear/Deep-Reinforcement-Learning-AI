using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{

    public Transform Target;
    private Rigidbody rigid;
    public float speed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        if(this.transform.position.y < 0)
        {
            this.rigid.angularVelocity = Vector3.zero;
            this.rigid.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }

        Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);

    }

    public override void CollectObservations()
    {
        AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);

        AddVectorObs(rigid.velocity.x);
        AddVectorObs(rigid.velocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        //Actions
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rigid.AddForce(controlSignal * speed);

        //Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        if(distanceToTarget < 1.42f)
        {
            SetReward(1f);
            Done();
        }

        //Reset if Agent Fell off
        if(transform.position.y < 0)
        {
            Done();
        }
    }

}
