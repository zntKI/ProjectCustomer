using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform car;

    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minVertAngle = -10f;
    [SerializeField] private float maxVertAngle = 30f;
    [SerializeField] private float minHorAngle = 50f;
    [SerializeField] private float maxHorAngle = 130f;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        float horizontalRotation = horizontalInput * rotationSpeed * Time.deltaTime;
        float verticalRotation = -verticalInput * rotationSpeed * Time.deltaTime;

        float oldVertRotation = transform.rotation.eulerAngles.x;
        float newVertRotation = oldVertRotation + verticalRotation;

        // clamp vertical rotation
        newVertRotation = ClampAngle(newVertRotation, minVertAngle, maxVertAngle);
        verticalRotation = newVertRotation - oldVertRotation;
        
        float oldHorRotation = transform.rotation.eulerAngles.y;
        float newHorRotation = oldHorRotation + horizontalRotation;

        float curMinHorAngle = minHorAngle + transform.parent.rotation.eulerAngles.y;
        curMinHorAngle = curMinHorAngle > 180 ? curMinHorAngle - 360 : curMinHorAngle;
        float curMaxHorAngle = maxHorAngle + transform.parent.rotation.eulerAngles.y;
        curMaxHorAngle = curMaxHorAngle > 180 ? curMaxHorAngle - 360 : curMaxHorAngle;

        newHorRotation = ClampAngle(newHorRotation, curMinHorAngle, curMaxHorAngle);
        horizontalRotation = newHorRotation - oldHorRotation;

        transform.Rotate(Vector3.up, horizontalRotation, Space.World);
        transform.Rotate(Vector3.right, verticalRotation);
    }


    //Clamps angle between two values
    //Accounts for negative degrees
    private float ClampAngle(float angle, float minRotation, float maxRotation)
    {
        float tempAngle = angle > 180 ? angle - 360 : angle;

        tempAngle = Mathf.Clamp(tempAngle, minRotation, maxRotation);

        if(tempAngle < 0)
        {
            tempAngle += 360;
        }

        return tempAngle;
    }
}
