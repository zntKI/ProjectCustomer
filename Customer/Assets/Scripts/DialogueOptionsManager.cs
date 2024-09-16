using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueOptionsManager : DialogueViewBase
{
    [SerializeField] GameObject optionsContainer;
    [SerializeField] GameObject buttonPrefab;

    private List<GameObject> instantiatedButtons = new List<GameObject>();

    // Called when Yarn Spinner has dialogue options to present

    public override void RunOptions(DialogueOption[] dialogueOptions, System.Action<int> onOptionSelected)
    {
        // Clear any previous buttons
        ClearOptions();

        // Create a button for each dialogue option
        foreach (var option in dialogueOptions)
        {
            GameObject newButton = Instantiate(buttonPrefab, optionsContainer.transform);
            Button buttonComponent = newButton.GetComponent<Button>();
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            newButton.layer = LayerMask.NameToLayer("DialogueOptionsLayer");
            BoxCollider collider = newButton.GetComponent<BoxCollider>();
            RectTransform rectTransform = optionsContainer.GetComponentInChildren<RectTransform>();

            // Set the text to the dialogue option's text
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
