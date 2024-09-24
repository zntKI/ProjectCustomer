using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class TutorialButtons : MonoBehaviour
{
    static Image DKey;

    private void Awake()
    {
        DKey = GetComponent<Image>();
    }

    [YarnCommand("enableTutorialImage")]
    public static void EnableImage()
    {
        DKey.enabled = true;
    }

    [YarnCommand("disableTutorialImage")]
    public static void DisableImage()
    {
        DKey.enabled = false;
    }
}
