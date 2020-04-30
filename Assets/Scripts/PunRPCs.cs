﻿using UnityEngine;
using Photon.Pun;

public class PunRPCs : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void WallWasHit(Vector3 cameraPos, Vector3 cameraForward, float weaponRange, int weaponDamage)
    {     
        RaycastHit hit;
        if (Physics.Raycast(cameraPos, cameraForward, out hit, weaponRange))
        {
            AgentInputHandler targetAgentInputHandler = hit.transform.gameObject.GetComponent<AgentInputHandler>();

            Debug.Log(targetAgentInputHandler);
            
            if (targetAgentInputHandler != null)
            {
                if (targetAgentInputHandler.runCommandOnAgentHasBeenHit != null)
                {
                    targetAgentInputHandler.runCommandOnAgentHasBeenHit(targetAgentInputHandler, hit.point, hit.normal, weaponDamage);
                }
            }
            else
            {
                Debug.LogWarning(hit.transform.gameObject.name + " has no agentInputHandler");
            }
        }
    }

    [PunRPC]
    public void PlayerWasHit(int hitPlayerViewID, Vector3 hitPos, Vector3 hitNormal, int weaponDamage)
    {
        AgentInputHandler targetAgentInputHandler = GetInputHandler(hitPlayerViewID);

        if (targetAgentInputHandler != null)
        {
            if (targetAgentInputHandler.runCommandOnAgentHasBeenHit != null)
                {
                    targetAgentInputHandler.runCommandOnAgentHasBeenHit(targetAgentInputHandler, hitPos, hitNormal, weaponDamage);
                }
        }
        else
        {
            Debug.LogWarning(targetAgentInputHandler.gameObject.name + " has no agentInputHandler");
        }
    }

    [PunRPC]
    public void Toggle(int playersViewID)
    {
        AgentInputHandler agentInputHandler = GetInputHandler(playersViewID);
        agentInputHandler.behaviourToToggle.enabled = !agentInputHandler.behaviourToToggle.isActiveAndEnabled;
    }

    [PunRPC]
    public void PlayGunshot(int agentsViewID)
    {
        AgentInputHandler agentInputHandler = GetInputHandler(agentsViewID);
        Debug.Log("PlayGunshot: Sending to all.");
        if (agentInputHandler.currentWeapon.weaponSound != null)
        {
            AudioSource weaponAudioSource = agentInputHandler.weaponObject.GetComponentInChildren<AudioSource>();

            if (weaponAudioSource == null)
            {
                weaponAudioSource = agentInputHandler.weaponObject.AddComponent(typeof(AudioSource)) as AudioSource;
            }

            weaponAudioSource.PlayOneShot(agentInputHandler.currentWeapon.weaponSound);
        } 
        else
        {
            Debug.LogAssertion(agentInputHandler.currentWeapon.name + " is missing a gunshot sound");
        }
    }

    [PunRPC]
    public void MuzzleFlash(int agentsViewID)
    {
        AgentInputHandler agentInputHandler = GetInputHandler(agentsViewID);
        Debug.Log("MuzzleFlash: Sending to all.");
        if (agentInputHandler.weaponMuzzleFlash != null)
        {
            agentInputHandler.weaponMuzzleFlash.Play();
        }
        else
        {
            Debug.LogAssertion(agentInputHandler.currentWeapon.name + " has no muzzle flash");
        }
    }

    private AgentInputHandler GetInputHandler(int viewId)
    {
        return PhotonNetwork.GetPhotonView(viewId).GetComponent<AgentController>();
    }
}
