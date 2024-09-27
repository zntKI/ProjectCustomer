using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TrafficLightController : MonoBehaviour
{
    public static event Action OnSignalChangeBackToGreen;

    [SerializeField]
    float timeRedLightSec = 10f;

    [SerializeField]
    Light lightToChange;

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
                lightToChange.color = Color.green;
                OnSignalChangeBackToGreen?.Invoke();
                break;
            case TrafficLightSignal.Red:
                lightToChange.color = Color.red;
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