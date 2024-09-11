using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkAIMovement : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed * Time.deltaTime);
    }
}
