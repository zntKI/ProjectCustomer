using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] DialogueRunner dialogueRunner;
    VariableStorageBehaviour variableStorageBehaviour;

    private void Start()
    {
        variableStorageBehaviour = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        
    }

    void Update()
    {
        /*float testVar;
        variableStorageBehaviour.TryGetValue("$testVar", out testVar);
        variableStorageBehaviour.SetValue("$testVar", testVar + 1);
        Debug.Log(testVar);*/
        if (Input.GetKeyDown(KeyCode.E)) {
            dialogueRunner.StartDialogue("Intro");
        }
    }

    [YarnCommand("testFunction")]
    public static void TestFunction()
    {
        Debug.Log("test function working");
    }
}
