using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class DialogueManager : DialogueViewBase
{
    public static DialogueManager instance;

    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] GameObject optionsContainer;
    [SerializeField] GameObject lastLine;
    [SerializeField] GameObject buttonPrefab;

    [SerializeField] float progressDelay = 2f;

    readonly private List<GameObject> instantiatedButtons = new();

    private Coroutine autoProgressOptionsCoroutine;
    private Coroutine autoProgressDialogueCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Called when Yarn Spinner has dialogue options to present
    public override void RunOptions(DialogueOption[] dialogueOptions, System.Action<int> onOptionSelected)
    {
        // Clear any previous buttons
        ClearOptionButtons();

        // Activate the options container so the buttons are visible

        optionsContainer.SetActive(true);
        // Display the last line said
        lastLine.SetActive(true);

        // Create a button for each dialogue option
        foreach (var option in dialogueOptions)
        {
            GameObject newButton = Instantiate(buttonPrefab, optionsContainer.transform);
            Button buttonComponent = newButton.GetComponent<Button>();
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            BoxCollider collider = newButton.GetComponent<BoxCollider>();
            RectTransform rectTransform = optionsContainer.GetComponentInChildren<RectTransform>();

            // Set the button text to the dialogue option's text
            buttonText.text = " ->     " + option.Line.Text.Text;

            //set collider size
            float optionWidth = rectTransform.rect.width;
            collider.size = new Vector3(optionWidth, collider.size.y, collider.size.z);
            collider.center = new Vector3(200, 0, 0);
            
            // Add an event listener to handle button clicks
            int optionIndex = option.DialogueOptionID;
            buttonComponent.onClick.AddListener(() => {
                onOptionSelected(optionIndex); // Call the callback to select the option
                ClearOptionButtons();                // Clear the options once one is selected
                StopAutoProgressOptions();        // Stop coroutine
                lastLine.SetActive(false);     // Clear the last line said
            });

            instantiatedButtons.Add(newButton);
        }

        autoProgressOptionsCoroutine = StartCoroutine(AutoProgressOptionsCoroutine(dialogueOptions.Length, onOptionSelected));

    }

    // Select the last option after a delay
    private IEnumerator AutoProgressOptionsCoroutine(int numberOfOptions, System.Action<int> selectOption)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(progressDelay);

        // Automatically choose the last option
        int lastOptionIndex = numberOfOptions - 1;
        instantiatedButtons[lastOptionIndex].GetComponent<Button>().onClick.Invoke();
    }

    // Clears all instantiated buttons
    private void ClearOptionButtons()
    {
        foreach (GameObject button in instantiatedButtons)
        {
            Destroy(button);
        }
        instantiatedButtons.Clear();
    }

    private void StopAutoProgressOptions()
    {
        // Stop the auto-progress coroutine if it's running
        if (autoProgressOptionsCoroutine != null)
        {
            StopCoroutine(autoProgressOptionsCoroutine);
            autoProgressOptionsCoroutine = null;
        }
    }


    /// <summary>
    /// Auto progress options
    /// </summary>
    ///

    //Called when Yarn Spinner has dialogue to present
    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        base.RunLine(dialogueLine, onDialogueLineFinished);

        // Start displaying the line
        autoProgressDialogueCoroutine = StartCoroutine(AutoProgressDialogueCoroutine());
    }

    private IEnumerator AutoProgressDialogueCoroutine()
    {
        float timeElapsed = 0f;
        bool waitingForInput = true;

        // Wait for either input or auto-progression
        while (waitingForInput && timeElapsed < progressDelay)
        {
            // If the player continues dialogue
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Stop waiting for input
                waitingForInput = false;
                StopAutoProgressDialogue();
                yield break;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (waitingForInput)
        {
            // Time ran out, auto-progress the dialogue
            waitingForInput = false;
            dialogueRunner.dialogueViews[0].UserRequestedViewAdvancement();
        }
    }

    private void StopAutoProgressDialogue()
    {
        // Stop the auto-progress coroutine if it's running
        if (autoProgressDialogueCoroutine != null)
        {
            StopCoroutine(autoProgressDialogueCoroutine);
            autoProgressDialogueCoroutine = null;
        }
    }
}
