using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CombatController : NetworkBehaviour
{

    public GameObject laserProjectilePrefab;
    public Transform shootPos;
    [SyncVar] public float health;
    public float maxHealth;
    public GameObject playerBody;
    public GameObject destroyedPrefab;
    private MovementController movementController;
    private float shootTC;
    public float firerate;

    [SyncVar] public bool isDead;

    
    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        setDead();
    }

    private void setDead()
    {
        isDead = true;
        playerBody.SetActive(false);
    }
    
   

    //Called on server
    public void DamagePlayer(float amount,NetworkIdentity sender)
    {

        if (isDead)
        {
            print("player already dead");
            return;
        }
        
        health -= amount;

        print("health = "+health);
        
        if (health<=0)
        {
            sender.GetComponent<PlayerInfo>().AddScore(100);
            PlayerDeath();
        }
        else
        {
            sender.GetComponent<PlayerInfo>().AddScore(10);
        }
    }
    
    //Called on server
    private void PlayerDeath()
    {
        isDead = true;
        RPCPlayerDeath();
        print("DEATH");
    }
    
    [ClientRpc]
    private void RPCPlayerDeath()
    {
        GameObject destroyed = Instantiate(destroyedPrefab, transform.position, transform.rotation);
        Rigidbody[] rigidbodies = destroyed.GetComponentsInChildren<Rigidbody>();
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.AddExplosionForce(800f,transform.position,10f);
        }

        if (isLocalPlayer)
        {
            UIManager.singleton.OpenTab("respawn");
        }
        
        playerBody.SetActive(false);
        GetComponent<MovementController>().PlayerDeath();
    }

    [Command]
    public void CMDPlayerRespawn()
    {
        isDead = false;
        print("Respawn");
        health = maxHealth;
        RPCPlayerRespawn();
    }

    
    
    
    [ClientRpc]
    private void RPCPlayerRespawn()
    {
        print("Respawn");

        NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();

        if (isLocalPlayer)
        {
            UIManager.singleton.CloseTab("respawn");
        }
        
        StartCoroutine(CoroutineMovePlayer(startPositions[Random.Range(0,startPositions.Length)].transform.position));
        playerBody.SetActive(true);
        GetComponent<MovementController>().enabled = true;
        GetComponent<MovementController>().PlayerRespawn();
    }
    
    public IEnumerator CoroutineMovePlayer(Vector3 pos)
    {
        for (int i = 0; i < 10; i++)
        {
            transform.position = pos;
            yield return null;
        }
    }
    
    private void Update()
    {
        if (isLocalPlayer)
        {

            UIManager.singleton.SetCrosshairPos((shootPos.position+shootPos.forward*20f));
            
            
            if (isShooting&&!UIManager.singleton.isPaused)
            {
                if (shootTC<=0)
                {
                    CMDShoot(shootPos.position,transform.rotation,GetComponent<Rigidbody>().velocity,netIdentity);
                    CameraController.singleton.Shoot();
                    movementController.AddRecoil(0.2f);
                    shootTC = firerate;
                }

                shootTC -= Time.deltaTime;
            }
            //CMDPlayerRespawn();
        }
        else
        {
            if (!isDead&& !playerBody.activeSelf)
            {
                playerBody.SetActive(true);
                GetComponent<MovementController>().enabled = true;
                GetComponent<MovementController>().PlayerRespawn();
            }
        }
    }

    public bool isShooting;
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed&&!isShooting)
        {
            shootTC = 0;
            isShooting = true;
        }else if (context.canceled)
        {
            isShooting = false;
        }
    }

    
    
    
    
    [Command]
    private void CMDShoot(Vector3 pos,Quaternion rot,Vector3 startVel,NetworkIdentity sender)
    {
        GameObject projectile = Instantiate(laserProjectilePrefab,pos,rot);
        projectile.GetComponent<LaserProjectile>().startVelocity = startVel;
        projectile.GetComponent<LaserProjectile>().sender = sender;
        NetworkServer.Spawn(projectile);
        RPCShoot();
    }

    [ClientRpc]
    private void RPCShoot()
    {
        GetComponent<AudioController>().PlayClip("laser");
    }

}
