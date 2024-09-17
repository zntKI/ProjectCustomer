using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueOptionsManager : DialogueViewBase
{
    public static DialogueOptionsManager instance;

    [SerializeField] GameObject optionsContainer;
    [SerializeField] GameObject lastLine;
    [SerializeField] GameObject buttonPrefab;

    private List<GameObject> instantiatedButtons = new List<GameObject>();

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
        ClearOptions();

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
            collider.center = new Vector3(optionWidth / 2 - 500, 0, 0);
            
            // Add an event listener to handle button clicks
            int optionIndex = option.DialogueOptionID;
            buttonComponent.onClick.AddListener(() => {
                onOptionSelected(optionIndex); // Call the callback to select the option
                ClearOptions();                // Clear the options once one is selected
                lastLine.SetActive(false);     // Clear the last line said
            });

            instantiatedButtons.Add(newButton);
        }

        // Activate the options container so the buttons are visible
        optionsContainer.SetActive(true);
    }

    // Clears all instantiated buttons
    private void ClearOptions()
    {
        foreach (GameObject button in instantiatedButtons)
        {
            Destroy(button);
        }
        instantiatedButtons.Clear();
    }
}
