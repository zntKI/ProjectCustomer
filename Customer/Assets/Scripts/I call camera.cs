using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icallcamera : MonoBehaviour
{
    public Camerashake cameraShake;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Camerashake.Shake(0.5f, 0.5f);
        }
    }
}
