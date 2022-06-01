using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
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
    }

    //Called on server
    public void DamagePlayer(float amount)
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
            PlayerDeath();
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
        
        playerBody.SetActive(false);
        GetComponent<MovementController>().PlayerDeath();
    }

    [Command]
    private void CMDPlayerRespawn()
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
            
            if (Input.GetMouseButtonDown(0))
            {
                shootTC = 0;
            }
            
            if (Input.GetMouseButton(0)&&!UIManager.singleton.isPaused&&!movementController.isLocked)
            {
                shootTC -= Time.deltaTime;
                if (shootTC<=0)
                {
                    CMDShoot(shootPos.position,transform.rotation,GetComponent<Rigidbody>().velocity,netIdentity);
                    CameraController.singleton.Shoot();
                    movementController.AddRecoil(0.2f);
                    shootTC = firerate;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                CMDPlayerRespawn();
            }
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