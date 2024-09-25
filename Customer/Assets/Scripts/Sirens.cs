using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sirens : MonoBehaviour
{
    public GameObject redLight, blueLight;
    public float waitTime = .2f;

    // Start is called before the first frame update
    void Start()
    {
StartCoroutine(Siren())
    }

    IEnumerator Siren()
    {
        yield return new WaitForSeconds(waitTime);

        redLight.SetActive(false);
        blueLight.SetActive(true); 
        
        yield return new WaitForSeconds(waitTime);


        redLight.SetActive(true);
        blueLight.SetActive(false);
    }


}
