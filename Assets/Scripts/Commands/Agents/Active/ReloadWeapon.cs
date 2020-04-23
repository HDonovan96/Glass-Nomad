﻿using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultReload", menuName = "Commands/Active/ReloadWeapon", order = 0)]
public class ReloadWeapon : ActiveCommandObject
{
    [SerializeField]
    private string reloadKey = "Reload";

    protected override void OnEnable()
    {
        keyTable.Add("Reload", reloadKey);
    }

    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnUpdate += RunCommandOnUpdate;
    }

    private void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        if (Input.GetKeyDown(reloadKey))
        {
            AgentController agentController = (AgentController)agentInputHandler;

            if (CanReload(agentController))
            {
                AudioSource weaponAudioSource = agentInputHandler.weaponObject.GetComponent<AudioSource>();
                Debug.Log(weaponAudioSource);
                weaponAudioSource.PlayOneShot(agentInputHandler.currentWeapon.reloadSound);

                agentInputHandler.StartCoroutine(Reload(agentInputHandler.currentWeapon.reloadDuration, agentController));

                agentInputHandler.isReloading = true;
            }
        }
    }

    private bool CanReload(AgentController agentController)
    {
        if (agentController.currentExtraAmmo > 0)
        {
            if (agentController.currentBulletsInMag < agentController.currentWeapon.magSize)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator Reload(float reloadTime, AgentController agentController)
    {
        yield return new WaitForSeconds(reloadTime);
        int bulletsUsed;

        bulletsUsed = agentController.currentWeapon.magSize - agentController.currentBulletsInMag;

        if (bulletsUsed > agentController.currentExtraAmmo)
        {
            bulletsUsed = agentController.currentExtraAmmo;
        }

        agentController.ChangeResourceCount(AgentController.ResourceType.MagazineAmmo, bulletsUsed);
        agentController.ChangeResourceCount(AgentController.ResourceType.ExtraAmmo, -bulletsUsed);

        agentController.isReloading = false;
    }
}