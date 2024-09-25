using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class TutorialButtons : MonoBehaviour
{
    [SerializeField] Image DKey;
    [SerializeField] Image SKey;

    [SerializeField] float _lerpSpeed = 4f;
    Color primaryColor = Color.white;
    Color secondaryColor = Color.gray;
    private float t;

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

    private void Update()
    {
        t += Time.deltaTime;

        if (DKey.enabled) {

            DKey.color = Color.Lerp(primaryColor, secondaryColor, Mathf.Abs(Mathf.Sin(t * _lerpSpeed)));
        }

        if (SKey.enabled)
        {
            SKey.color = Color.Lerp(primaryColor, secondaryColor, Mathf.Abs(Mathf.Sin(t * _lerpSpeed)));
        }
    }
}
