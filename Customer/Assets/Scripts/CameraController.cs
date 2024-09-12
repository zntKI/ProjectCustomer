using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minVertAngle = 0f;
    [SerializeField] private float maxVertAngle = 30f;
    [SerializeField] private float minHorAngle = 20f;
    [SerializeField] private float maxHorAngle = 310f;

    private void Start()
    {
        Cursor.visible = false;
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
        newVertRotation = Mathf.Clamp(newVertRotation, minVertAngle, maxVertAngle);
        verticalRotation = newVertRotation - oldVertRotation;
        
        float oldHorRotation = transform.rotation.eulerAngles.y;
        float newHorRotation = oldHorRotation + horizontalRotation;

        // clamp horizontal rotation
        // clamp between 310-360 or 0-50
        if (newHorRotation > minHorAngle && newHorRotation < maxHorAngle)
        {
            // if the new rotation is outside of both ranges, choose the closest limit
            if (oldHorRotation <= minHorAngle)
            {
                newHorRotation = minHorAngle - 0.1f;  // Stay within the lower range
            }
            else if (oldHorRotation >= maxHorAngle)
            {
                newHorRotation = maxHorAngle + 0.1f;  // Stay within the upper range
            }
        }
        
        horizontalRotation = newHorRotation - oldHorRotation;


        transform.Rotate(Vector3.up, horizontalRotation, Space.World);
        transform.Rotate(Vector3.right, verticalRotation);
    }
}
