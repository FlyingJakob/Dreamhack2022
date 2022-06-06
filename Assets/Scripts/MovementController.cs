using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
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

    
    #region Input
    private PlayerControls playerControls;
    public Vector2 rotationInput;
    public Vector2 moveInput;
    
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
    
    

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        InitInputs();
        PlayerDeath();
    }

    private void InitInputs()
    {
        playerControls = new PlayerControls();
        playerControls.GamePlay.Rotate.performed += ctx => rotationInput = ctx.ReadValue<Vector2>();
        playerControls.GamePlay.Rotate.canceled += ctx => rotationInput = Vector2.zero;
        playerControls.GamePlay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.GamePlay.Move.canceled += ctx => moveInput = Vector2.zero;
    }
    
    private void OnEnable()
    {
        playerControls.GamePlay.Enable();
    }

    private void OnDisable()
    {
        playerControls.GamePlay.Disable();
    }


    
    private void PlayerRotation()
    {
        if (UIManager.singleton.isPaused||isLocked) { return; }
        float mouseX = rotationInput.x;
        float mouseY = rotationInput.y;
        float steeringSens = Mathf.Clamp(rigidbody.velocity.magnitude * 0.2f,0.1f,1f);
        
        rigidbody.AddTorque(((transform.up*mouseX-transform.right*mouseY)*rotationSpeed - (transform.forward * moveInput.x * 3f))*steeringSens*Time.deltaTime*800f);
        
        
        
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
        rigidbody.AddForce(transform.forward*moveInput.y * thrusterForce*Time.deltaTime);
        currentThrust = Mathf.Lerp(currentThrust, Mathf.Clamp(moveInput.y,0,1), Time.deltaTime * 10f);
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

