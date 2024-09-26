using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonCarControl : MonoBehaviour
{
    public Animator carAnimator;  // Reference to the Animator controlling the car
    public float stopTime = 2f;   // Time for the car to stop before transitioning

    private bool isMovingRight;   // Track whether the car is moving left-to-right

    void Start()
    {
        // Assume the car starts moving left to right
        isMovingRight = true;
    }

    public void OnPlayButtonPressed()
    {
        // Only stop the car if it's currently moving left-to-right
        if (isMovingRight)
        {
            // Trigger the car stop animation
            carAnimator.SetTrigger("StopGreyCar");

            // Invoke the scene load after a delay
            Invoke("LoadGameScene", stopTime);
        }
    }

    private void LoadGameScene()
    {
        // Load the game scene (replace "GameScene" with your scene's name)
        SceneManager.LoadScene(1);
    }

    // Function to update whether the car is moving left or right
    public void SetCarMovingRight(bool movingRight)
    {
        isMovingRight = movingRight;
    }
}
