using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaycastDialogueSelector : MonoBehaviour
{
    Camera playerCamera;               // The camera from which the ray will be cast
    public float raycastDistance = 100f;      // Max raycast distance
    public Color highlightColor = Color.yellow;  // Highlight color for selected buttons
    public LayerMask interactableLayer;       // Layer for raycastable dialogue options

    private Button highlightedButton = null;
    private Color originalColor;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            Button button = hit.transform.GetComponent<Button>();
            

            if (button != null)
            {
                HighlightButton(button);

                // Check if the player clicks the mouse or presses a selection key
                if (Input.GetMouseButtonDown(0))
                {
                    button.onClick.Invoke();  // Invoke the button click
                }
            }
        }
        else
        {
            // Clear highlight if no button is hit
            ClearHighlight();
        }
    }

    private void HighlightButton(Button button)
    {
        if (highlightedButton != button)
        {
            ClearHighlight();  // Clear previously highlighted button

            // Save the original color and apply the highlight
            var colors = button.colors;
            originalColor = colors.normalColor;
            colors.normalColor = highlightColor;
            
            button.colors = colors;
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = Color.white;

            highlightedButton = button;
        }
    }

    private void ClearHighlight()
    {
        if (highlightedButton != null)
        {
            // Reset the original color
            var colors = highlightedButton.colors;
            colors.normalColor = originalColor;
            highlightedButton.colors = colors;

            TextMeshProUGUI buttonText = highlightedButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = Color.gray;

            highlightedButton = null;
        }
    }
}