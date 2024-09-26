using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScenemanager : MonoBehaviour
{
    
    public float stopTime = 2f;   // Time for the car to stop before transitioning

  


    public void OnPlayButtonPressed()
    {
        // Only stop the car if it's currently moving left-to-right
        
        
            

            // Invoke the scene load after a delay
            Invoke("LoadGameScene", stopTime);
        
    }

    private void LoadGameScene()
    {
        // Load the game scene (replace "GameScene" with your scene's name)
        SceneManager.LoadScene("LEVEL 1");
    }

    // Function to update whether the car is moving left or right
  
}
