using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public float attackSpeed = 0.75f;
    private float maxShootingDistance = 12.5f;
    private float waitingTime;
    public bool canFire = true;
    public int m_PlayerNumber;

    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
 
    private void Start()
    {
        waitingTime = attackSpeed;
    }

    private void Update()
    {
        if (!canFire)
        {
            waitingTime -= Time.deltaTime;
            if (waitingTime <= 0) canFire = true;
        }
    }


    public void Fire(float launchForce)
    {
        canFire = false;
        waitingTime = attackSpeed;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        if (launchForce > maxShootingDistance) launchForce = maxShootingDistance;
        shellInstance.velocity = launchForce * m_FireTransform.forward;

    }
}