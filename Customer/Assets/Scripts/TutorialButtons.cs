using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class TutorialButtons : MonoBehaviour
{
    [SerializeField] Image DKey;
    [SerializeField] Image SKey;

    [YarnCommand("enableDKey")]
    public void EnableDKey()
    {
        DKey.enabled = true;
    }

    [YarnCommand("disableDKey")]
    public  void DisableDKey()
    {
        DKey.enabled = false;
    }

    [YarnCommand("enableSKey")]
    public void EnableSKey()
    {
        SKey.enabled = true;
    }

    [YarnCommand("disableSKey")]
    public void DisableSKey()
    {
        SKey.enabled = false;
    }
}
