using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkAIMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float targetMoveSpeed = 75f;
    [SerializeField]
    float moveSpeedIncreaseAmount = 5f;

    [Header("Swerving")]
    [SerializeField]
    float swerveMultiplierMin;
    [SerializeField]
    float swerveMultiplierMax;
    [SerializeField]
    float swerveRotationDefaultAmount = 0.1f;
    [SerializeField]
    float playerTurnAmount = 2f;

    MovementState state;

    Rigidbody rb;

    float moveSpeed;

    float swerveInputValue;
    float currentSwerveMultipier;
    float rotationWhenStartedSwerving;

    private void Awake()
    {
        state = MovementState.Start;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleState();
    }


    void FixedUpdate()
    {
        switch (state)
        {
            case MovementState.Swerving:

                float swerveRotationAmount = swerveRotationDefaultAmount * currentSwerveMultipier; //swerve force
                float playerRotationAmount = Mathf.Abs(swerveRotationAmount) * 2 * swerveInputValue; //player force
                float rotationAmount = swerveRotationAmount + playerRotationAmount; //end force amount depending on input
                
                transform.Rotate(0f, rotationAmount, 0f);

                bool hasCarReturnedToStartingRotation = currentSwerveMultipier < 0 ? transform.rotation.y > rotationWhenStartedSwerving
                    : transform.rotation.y < rotationWhenStartedSwerving;
                if (hasCarReturnedToStartingRotation)
                {
                    SetState(MovementState.PassengerControl);
                }

                break;
            case MovementState.PassengerControl:

                transform.Rotate(0f, swerveInputValue * playerTurnAmount, 0f);

                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }

        rb.velocity = transform.forward * moveSpeed;
    }

    void HandleState()
    {
        switch (state)
        {
            case MovementState.Start:
                moveSpeed += moveSpeedIncreaseAmount * Time.deltaTime;
                if (moveSpeed >= targetMoveSpeed)
                {
                    moveSpeed = targetMoveSpeed;
                    SetState(MovementState.Swerving/*(MovementState)Random.Range(2, 4)*/);
                }
                break;
            case MovementState.Swerving:
                HandlePlayerInput();
                break;
            case MovementState.PassengerControl:
                HandlePlayerInput(); //TODO: until you reach a waypoint!!!
                break;
            default:
                break;
        }
    }

    void HandlePlayerInput()
    {
        swerveInputValue = Input.GetAxisRaw("Horizontal");
    }

    void SetState(MovementState stateToChangeTo)
    {
        state = stateToChangeTo;
        switch (state)
        {
            case MovementState.Start:
                break;
            case MovementState.Swerving:
                rotationWhenStartedSwerving = transform.rotation.y;
                currentSwerveMultipier = Random.Range(swerveMultiplierMin, swerveMultiplierMax);
                break;
            case MovementState.PassengerControl:
                break;
            case MovementState.Accelerating:
                break;
            default:
                break;
        }
    }
}


public enum MovementState
{
    Start,
    PassengerControl,
    Swerving,
    Accelerating
}