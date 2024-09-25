using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        DrunkAIMovement.onMovespeedChange += UpdateSpeedometerText;
        text = GetComponent<TextMeshProUGUI>();
    }

    void UpdateSpeedometerText(float speed)
    {
        text.text = ((int)speed * 5).ToString();
    }

    private void OnDestroy()
    {
        DrunkAIMovement.onMovespeedChange-= UpdateSpeedometerText;
    }
}
