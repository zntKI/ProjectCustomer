using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 30f;

    private void Start()
    {
        //Cursor.visible = false;
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        float horizontalRotation = horizontalInput * rotationSpeed * Time.deltaTime;
        float verticalRotation = -verticalInput * rotationSpeed * Time.deltaTime;

        // clamp vertical rotation
        float oldRotation = transform.rotation.eulerAngles.x;
        float newRotation = oldRotation + verticalRotation;
        newRotation = Mathf.Clamp(newRotation, minAngle, maxAngle);
        verticalRotation = newRotation - oldRotation;

        transform.Rotate(Vector3.up, horizontalRotation, Space.World);
        transform.Rotate(Vector3.right, verticalRotation);
    }
}
