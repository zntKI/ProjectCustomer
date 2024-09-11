using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkAIMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float moveSpeed = 1f;

    [Header("Swerving")]
    [SerializeField]
    float swerveTimerMinSec;
    [SerializeField]
    float swerveTimerMaxSec;
    [SerializeField]
    float swerveMultiplierMin;
    [SerializeField]
    float swerveMultiplierMax;
    [SerializeField]
    float swerveRotationDefaultAmount = 0.1f;

    float swerveTimer;
    float currentSwerveCounter;
    float currentSwerveMultipier;


    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SetSwerveTimer();
    }

    void Update()
    {
        currentSwerveCounter += Time.deltaTime;
        if (currentSwerveCounter >= swerveTimer)
        {
            currentSwerveMultipier = Random.Range(swerveMultiplierMin, swerveMultiplierMax);

            //Debug.Log($"SwerveMultiplier amount: {currentSwerveCounter}");

            SetSwerveTimer();
        }
    }

    void FixedUpdate()
    {
        transform.Rotate(0f, swerveRotationDefaultAmount * currentSwerveMultipier, 0f);
        rb.velocity = transform.forward * moveSpeed;
    }


    void SetSwerveTimer()
    {
        swerveTimer = Random.Range(swerveTimerMinSec, swerveTimerMaxSec);
        currentSwerveCounter = 0f;

        //Debug.Log($"Swerve timer set: {swerveTimer}");
    }
}
