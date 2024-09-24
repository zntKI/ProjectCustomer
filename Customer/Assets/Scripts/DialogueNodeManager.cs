using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueNodeManager : MonoBehaviour
{
    public static DialogueNodeManager instance;

    [SerializeField] DialogueRunner dialogueRunner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //Start with the first dialogue node
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            dialogueRunner.StartDialogue("Intro");
        }
    }

    // Start selected dialogue node
    public void StartDialogue(string startNode)
    {
        dialogueRunner.StartDialogue(startNode);
    }

    // Return current dialogue node
    public string GetCurrentNode()
    {
        return dialogueRunner.CurrentNodeName;
    }

}
