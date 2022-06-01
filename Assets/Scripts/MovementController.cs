using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    #region Variables
    public float thrusterForce; //Hur stark raketmotorn är
    public float rotationSpeed; //Hur snabt spelaren roterar
    #endregion
    #region Components
    private Rigidbody rigidbody; //Rigidbody komponenten fäst till gameobjectet
    public List<Thruster> thrusters;
    public bool isLocked;
    public float liftAmount;
    #endregion

    #region LocalVariables
    [SyncVar] private float currentThrust;
    #endregion
    private void Update()
    {
        //Om denna spelare är den spelaren JAG styr
        if (isLocalPlayer)
        {
            Movement(); // Kallar på metoden där spelarens rörelse görs.
            PlayerRotation(); // Kallar på metoden där spelarens rotation ändras.
            ApplyLift();
        }
        SetThrust();
    }

    private void ApplyLift()
    {

        Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

        if (localVelocity.magnitude<4f)
        {
            return;
        }
        
        localVelocity.x = 0;

        float lift = Vector3.SignedAngle(localVelocity, Vector3.forward,Vector3.right);
        
        print(lift*localVelocity.magnitude);
        rigidbody.AddForce(-transform.up*lift*localVelocity.magnitude*Time.deltaTime*liftAmount);
    }

    [Command]
    private void CMDSetThrust(float amount)
    {
        currentThrust = amount;
    }
    private void SetThrust()
    {
        foreach (Thruster thruster in thrusters)
        {
            thruster.SetThrust(currentThrust);
        }
    }
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        PlayerRespawn();
    }
    
    //Här behandlar vi all rotation av spelaren
    private void PlayerRotation()
    {
        if (UIManager.singleton.isPaused||isLocked) { return; }
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float roll = Convert.ToInt32(Input.GetKey(KeyCode.Q))-Convert.ToInt32(Input.GetKey(KeyCode.E));
        float steeringSens = Mathf.Clamp(rigidbody.velocity.magnitude * 0.2f,0.1f,1f);
        rigidbody.AddTorque(((transform.up*mouseX-transform.right*mouseY)*rotationSpeed+ (transform.forward * roll * 3f*Time.deltaTime*800f))*steeringSens);
    }
    //Här behandlar vi all rörelse av spelaren
    private void Movement()
    {
        if (UIManager.singleton.isPaused||isLocked)
        {
            currentThrust = Mathf.Lerp(currentThrust, 0, Time.deltaTime * 8f);
            CMDSetThrust(currentThrust);
            return;
        }
        
        if (Input.GetKey(KeyCode.W)) // Om spelaren trycker på knappen 'W'
        {
            rigidbody.AddForce(transform.forward *
                               thrusterForce*Time.deltaTime); //Lägger till en kraft framåt på raketen av storleken thrusterForce.
            currentThrust = Mathf.Lerp(currentThrust, 1, Time.deltaTime * 8f);
        }
        else
        {
            currentThrust = Mathf.Lerp(currentThrust, 0, Time.deltaTime * 8f);
        }

        if (Input.GetKey(KeyCode.S)) // Om spelaren trycker på knappen 'W'
        {
            rigidbody.AddForce(-transform.forward *
                               thrusterForce*0.5f*Time.deltaTime); //Lägger till en kraft bakåt på raketen av storleken thrusterForce.
            
        }
        CMDSetThrust(currentThrust);
    }

    public void PlayerRespawn()
    {
        print("repsawn");
        foreach (Thruster thruster in thrusters)
        {
            thruster.gameObject.SetActive(true);
        }

        isLocked = false;
        rigidbody.isKinematic = false;
    }

    public void AddRecoil(float amount)
    {
        rigidbody.AddForce(-transform.forward*amount,ForceMode.Impulse);
    }
    
    
    public void PlayerDeath()
    {
        foreach (Thruster thruster in thrusters)
        {
            thruster.gameObject.SetActive(false);
        }
        rigidbody.isKinematic = true;
        isLocked = true;

    }
    
    
    
    
    
}

