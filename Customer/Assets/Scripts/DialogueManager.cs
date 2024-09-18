using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] DialogueRunner dialogueRunner;
    VariableStorageBehaviour variableStorageBehaviour;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        variableStorageBehaviour = GameObject.FindObjectOfType<InMemoryVariableStorage>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            dialogueRunner.StartDialogue("Intro");
        }
    }

    public void StartDialogue(string startNode)
    {
        dialogueRunner.StartDialogue(startNode);
    }

    public string GetCurrentNode()
    {
        return dialogueRunner.CurrentNodeName;
    }
}
