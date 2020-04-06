﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerInteractionScript : MonoBehaviourPunCallbacks
{
    [Header("Trigger Interaction")]
    [SerializeField] protected KeyCode inputKey = KeyCode.E; // Which key the player needs to be pressing to interact.
    [SerializeField] protected float interactTime; // The time needed to interact with the object to activate/open it.
    protected float currInteractTime = 0f; // How long the player has been pressing the interact key.
    [SerializeField] protected float cooldownTime; // How long it takes for the player to interact with the object again.
    protected float currCooldownTime = 0f; // How long it has been since the player last interacted with the object.
    protected bool interactionComplete = false; // Is the interaction complete?
    [SerializeField] protected bool debug = false; // Should the debug messages be displayed.
    protected Image outerReticle = null; // The reticle to display the progress of the interaction.
    protected TMP_Text interactionText = null; // The text component for when the player enter the object's collider.
    [SerializeField] protected string textToDisplay = "Hold E to interact"; // The text to appear when the player has entered the object's collider.
    protected GameObject playerInteracting = null;

    /// <summary>
    /// Constantly decreases the current cooldown time, unless its already 0.
    /// </summary>
    protected void Update()
    {
        if (currCooldownTime > 0)
        {
            currCooldownTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Upon entering the object's collider, attempt to retreive the outer reticle and interaction text from the player.
    /// If these are both successful, then the text is set to the 'textToDisplay'.
    /// </summary>
    /// <param name="coll"></param>
    protected virtual void OnTriggerEnter(Collider coll)
    {
        try {
            playerInteracting = coll.gameObject;
            
            if (coll.GetComponent<PhotonView>().IsMine)
            {
                if (debug) Debug.Log("PLAYER: " + playerInteracting.name);
                outerReticle = playerInteracting.GetComponent<AgentController>().transform.GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
                interactionText = playerInteracting.GetComponent<AgentController>().transform.GetChild(2).GetChild(1).GetChild(0).GetChild(3).GetComponent<TMP_Text>();
                interactionText.text = textToDisplay;
            }
        }
        catch {
            Debug.LogError("Outer Reticle or Interaction Text has not been set correctly.");
        }
    }

    /// <summary>
    /// Checks it is a player interacting with the object, the cooldown has worn off, the interaction
    /// isn't already completed, and the player is pressing the interaction key. This adds to the 
    /// current interaction timer. If this is equal or greater than the required interaction time,
    /// then the interaction is marked as complete, the current interaction timer gets set to 0, the
    /// cooldown is reset and the 'InteractionComplete' method is called.
    /// 'InteractionComplete' is an abstract method, meaning that all sub classes of this class are
    /// required to have an implementation of this method.
    /// </summary>
    /// <param name="coll"></param>
    protected void OnTriggerStay(Collider coll)
    {
        if (!coll.GetComponent<PhotonView>().IsMine) return;
        playerInteracting = coll.gameObject;
        
        if (playerInteracting.tag == "Player" && currCooldownTime <= 0 && !interactionComplete)
        {            
            if (Input.GetKey(inputKey) || inputKey == KeyCode.None)
            {
                if (currInteractTime >= interactTime)
                {
                    photonView.RPC("InteractionComplete", RpcTarget.All);
                    currInteractTime = 0f;
                    interactionComplete = true;
                    currCooldownTime = cooldownTime;
                    playerInteracting.GetComponent<AgentInputHandler>().allowInput = true;
                    return;
                }

                currInteractTime += Time.deltaTime;
                float percentage = (currInteractTime / interactTime) * 100;
                if (debug) Debug.LogFormat("Interaction progress: {0}%", percentage);
                
                ReticleProgress.UpdateReticleProgress(percentage, outerReticle);
                playerInteracting.GetComponent<AgentInputHandler>().allowInput = false;
                return;
            }

            LeftTriggerArea();
            interactionText.text = textToDisplay;
        }

        if (coll.tag == "Player" && interactionComplete)
        {
            coll.GetComponent<AgentInputHandler>().allowInput = true;
        }

        if (debug) Debug.LogFormat("Cooldown: {0} seconds", currCooldownTime);
    }

    /// <summary>
    /// If this player leaves the trigger area, then the current interaction timer is set back to 0,
    /// the interaction is marked as incomplete and the 'LeftTriggerArea' method is called.
    /// 'LeftTriggerArea' is a virtual method meaning that sub classes can (but don't have to) have
    /// an implementation of this method.
    /// </summary>
    /// <param name="coll"></param>
    protected void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject == playerInteracting)
        {
            interactionComplete = false;
            interactionText.text = "";
            playerInteracting.GetComponent<AgentInputHandler>().allowInput = true;
            playerInteracting = null;
            LeftTriggerArea();
        }
    }

    /// <summary>
    /// Upon completing the interaction, if the object's result is required to sync across network,
    /// then this rpc is called.
    /// </summary>
    [PunRPC]
    protected virtual void InteractionComplete()
    {
        GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// If the player isn't pressing the interaction key, or leaves the object's collider, then the
    /// functionality inside this method - which, again, may be overriden - is executed.
    /// As the base method, it resets the current interaction time, the interaction text, the reticle's
    /// progress and enables the player's input.
    /// </summary>
    /// <param name="coll"></param>
    protected virtual void LeftTriggerArea()
    {
        currInteractTime = 0f;
        ReticleProgress.UpdateReticleProgress(0, outerReticle);
    }
}
