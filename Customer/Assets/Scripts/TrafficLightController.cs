using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public static event Action OnSignalChangeBackToGreen;

    [SerializeField]
    float timeRedLightSec = 10f;

    [SerializeField]
    Material greenMat;
    [SerializeField]
    Material redMat;

    TrafficLightSignal signal;

    float timeRedLightSecCounter = 0f;

    void Update()
    {
        switch (signal)
        {
            case TrafficLightSignal.Green:
                break;
            case TrafficLightSignal.Red:
                timeRedLightSecCounter += Time.deltaTime;
                if (timeRedLightSecCounter > timeRedLightSec)
                {
                    ChangeSignal(TrafficLightSignal.Green);
                }
                break;
            default:
                break;
        }
    }

    public void ChangeSignal(TrafficLightSignal signal)
    {
        this.signal = signal;
        switch (signal)
        {
            case TrafficLightSignal.Green:
                GetComponent<MeshRenderer>().material = greenMat;
                OnSignalChangeBackToGreen?.Invoke();
                break;
            case TrafficLightSignal.Red:
                GetComponent<MeshRenderer>().material = redMat;
                timeRedLightSecCounter = 0;
                break;
            default:
                break;
        }
    }
}

public enum TrafficLightSignal
{
    Green,
    Red
}