﻿using UnityEngine;
using Photon.Pun;

using TMPro;

public class AgentInputHandler : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenu;
    public Behaviour behaviourToToggle;
    public AgentValues agentValues;
    public ActiveCommandObject[] activeCommands;
    public PassiveCommandObject[] passiveCommands;

    [Header("Movement")]
    public bool isSprinting = false;
    public bool isGrounded = true;
    public Vector3 gravityDirection = Vector3.down;
    public bool allowInput = true;
    public float currentLeapCharge = 0.0f;

    [Header("Oxygen")]
    public float currentOxygen = 0.0f;
    public GameObject oxygenDisplay;

    [Header("Weapons")]
    public Weapon currentWeapon;
    public int currentBulletsInMag = 0;
    public float timeSinceLastShot = 0.0f;
    public int currentTotalAmmo = 0;
    public float currentRecoilValue = 0.0f;
    public GameObject weaponObject;
    public ParticleSystem weaponMuzzleFlash;

    [Header("Camera")]
    public Camera agentCamera;

    [Header("Health")]
    public float currentHealth = 0.0f;

    [Header("UI")]
    public TextMeshProUGUI healthUIText;
    public TextMeshProUGUI ammoUIText;

    [Header("Agent Hit Feedback")]
    public AudioClip agentHitSound;
    public ParticleSystem agentHitParticles;

    [Header("PUN")]
    public PunRPCs punRPCs;
    
    protected GameObject agent;

    // Delegates used by commands.
    // Should add a delegate for UpdateUI(GameObject UIToUpdate, float newValue = 0.0f, int newIntValue = 0), maybe.
    public delegate void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues);
    public RunCommandOnUpdate runCommandOnUpdate;
    public delegate void RunCommandOnFixedUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues);
    public RunCommandOnFixedUpdate runCommandOnFixedUpdate;
    public delegate void RunCommandOnCollisionEnter(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues, Collision other);
    public RunCommandOnCollisionStay runCommandOnCollisionEnter;
    public delegate void RunCommandOnCollisionStay(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues, Collision other);
    public RunCommandOnCollisionStay runCommandOnCollisionStay;
    public delegate void RunCommandOnCollisionExit(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues, Collision other);
    public RunCommandOnCollisionExit runCommandOnCollisionExit;

    public delegate void RunCommandOnWeaponFired(AgentInputHandler agentInputHandler);
    public RunCommandOnWeaponFired runCommandOnWeaponFired;

    public delegate void RunCommandOnAgentHasBeenHit(AgentInputHandler agentInputHandler, Vector3 position, float value);
    public RunCommandOnAgentHasBeenHit runCommandOnAgentHasBeenHit;


    private void Start()
    {
        InitiliseVariable();

        InitiliseUI();

        foreach (ActiveCommandObject element in activeCommands)
        {
            element.RunCommandOnStart(this);
        }
        foreach (PassiveCommandObject element in passiveCommands)
        {
            element.RunCommandOnStart(this);
        }
    }

    private void Update()
    {
        if (runCommandOnUpdate != null)
        {
            runCommandOnUpdate(agent, this, agentValues);
        }
    }

    private void FixedUpdate()
    {
        if (runCommandOnFixedUpdate != null)
        {
            runCommandOnFixedUpdate(agent, this, agentValues);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (runCommandOnCollisionEnter != null)
        {
            runCommandOnCollisionEnter(agent, this, agentValues, other);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (runCommandOnCollisionStay != null)
        {
            runCommandOnCollisionStay(agent, this, agentValues, other);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (runCommandOnCollisionExit != null)
        {
            runCommandOnCollisionExit(agent, this, agentValues, other);
        }
    }

    void InitiliseVariable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        agent = this.gameObject;
        currentOxygen = agentValues.maxOxygen;
        currentHealth = agentValues.maxHealth;

        if (currentWeapon != null)
        {
            currentBulletsInMag = currentWeapon.bulletsInCurrentMag;
            currentTotalAmmo = currentWeapon.magSize * 3;
            timeSinceLastShot = currentWeapon.fireRate;
        }
    }

    void InitiliseUI()
    {
        if (healthUIText != null)
        {
            healthUIText.text = "Health: " + Mathf.RoundToInt(currentHealth / agentValues.maxHealth * 100);
        }
 
        if (ammoUIText != null)
        {
            ammoUIText.text = "Ammo: " + currentBulletsInMag + " / " + currentTotalAmmo;
        }

        
    }

    // public ParticleSystemStarted(ParticleSystem particleSystem)
    // {
    //     StartCoroutine(DisableParticlesWhenFinished);
    // }
}