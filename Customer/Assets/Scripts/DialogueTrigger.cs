using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueRunner dialogueRunner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            dialogueRunner.StartDialogue("Intro");
        }
    }
}
