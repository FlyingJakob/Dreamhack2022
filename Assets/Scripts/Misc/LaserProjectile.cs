using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LaserProjectile : NetworkBehaviour
{

    public float speed;
    [SyncVar] public bool hasHit;
    public LayerMask layerMask;
    public float damage;
    [SyncVar] public NetworkIdentity sender;
    [SyncVar] public Vector3 startVelocity;
    
    private void Update()
    {
        if (hasHit)
        {
            GetComponentInChildren<AudioSource>().volume =
                Mathf.Lerp(GetComponentInChildren<AudioSource>().volume, 0, Time.deltaTime * 100f);
        }
        if (hasHit||!isServer) { return; }
        transform.Translate((transform.forward * speed+startVelocity) * Time.deltaTime,Space.World);
        CheckHit();
    }
    private void CheckHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,speed*1.2f*Time.deltaTime,layerMask))
        {
            CombatController combatController = hit.collider.gameObject.GetComponentInParent<CombatController>();
            if (combatController!=null)
            {
                combatController.DamagePlayer(damage,sender);
            }
            hasHit = true;
            RPCHit();
        }
    }
    [ClientRpc]
    private void RPCHit()
    {
        if (sender==NetworkClient.localPlayer)
        {
            UIManager.singleton.MarkHit();
        }
    }
}
