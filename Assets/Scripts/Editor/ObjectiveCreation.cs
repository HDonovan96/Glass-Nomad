﻿using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class Objectives : EditorWindow
{    
    int chosenTrigger = 0;
    Button button;

    [MenuItem("Window/Dev Tools/Objective Creation")]
    public static void ShowWindow()
    {
        GetWindow<Objectives>("Objective Creation");
    }

    void OnGUI()
    {
        Triggers();
        Effects();
    }

    // Renders menu items handling the selection of triggers.
    void Triggers()
    {
        string[] triggerOptions = 
        {
            "On Button Press",
            "On Bool is True",
            "On Area Entered"
        };

        chosenTrigger = EditorGUI.Popup
        (
            new Rect(0, 0, 270, 20), 
            "Trigger:",
            chosenTrigger,
            triggerOptions
        );

        switch (chosenTrigger)
        {
            case 0:
                button = (Button)EditorGUILayout.ObjectField("Trigger Button", button, typeof(Button), true);
                return;

            default:
                return;
        }
    }

    // Renders menu items handling the effect of the trigger.
    void Effects()
    {

    }
}
