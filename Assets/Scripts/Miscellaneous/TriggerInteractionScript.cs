﻿using UnityEngine;

abstract public class TriggerInteractionScript : MonoBehaviour
{
    [SerializeField] protected KeyCode inputKey = KeyCode.E; // Which key the player needs to be pressing to interact.
    [SerializeField] protected float interactTime; // The time needed to interact with the object to activate/open it.
    protected float currInteractTime = 0f; // How long the player has been pressing the interact key.
    [SerializeField] protected float cooldownTime; // How long it takes for the player to interact with the object again.
    protected float currCooldownTime = 0f; // How long it has been since the player last interacted with the object.
    protected bool interactionComplete = false; // Is the interaction complete?


    /// <summary>
    /// Constantly decreases the current cooldown time, unless its already 0.
    /// </summary>
    protected void Update()
    {
        if (currCooldownTime >= 0)
        {
            currCooldownTime -= Time.deltaTime;
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
        if (coll.tag == "Player" && currCooldownTime <= 0 && !interactionComplete)
        {
            if (Input.GetKey(inputKey))
            {
                if (currInteractTime >= interactTime)
                {
                    InteractionComplete(coll.gameObject);
                    currInteractTime = 0f;
                    interactionComplete = true;
                    currCooldownTime = cooldownTime;
                }

                currInteractTime += Time.deltaTime;
                Debug.LogFormat("Interaction progress: {0}%", (currInteractTime / interactTime) * 100);
                return;
            }
            currInteractTime = 0f;
            return;
        }

        Debug.LogFormat("Cooldown: {0} seconds", currCooldownTime);
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
        if (coll.tag == "Player")
        {
            currInteractTime = 0f;
            interactionComplete = false;
            LeftTriggerArea();
        }
    }

    abstract protected void InteractionComplete(GameObject player);
    virtual protected void LeftTriggerArea() { }
}
