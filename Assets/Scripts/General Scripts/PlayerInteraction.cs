﻿using UnityEngine;

public class PlayerInteraction : ObjectInteraction
{
    public new InteractionType interactionType;

    public void ProcessTriggers(float deltaTime)
    {
        // Script to execute if interacting with a door.
        if (interactionType == InteractionType.Door)
        {
            float actionDuration = 2;

            Debug.Log(deltaTime);

            if (deltaTime >= actionDuration)
            {
                Debug.Log("Door Open");
            }
        }
    }
}
