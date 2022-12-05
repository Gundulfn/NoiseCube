using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public Transform groundChecker;
    public LayerMask groundMask;
    public GameObject _camera;

    public float walkSpeed = 10f;
    public float currentSpeed;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    private Vector3 velocity;
    private float groundDistance = 0.4f;
    private bool isGrounded = true;

    void Start()
    {
        _camera = GameObject.Find("Camera");
        _camera.transform.parent = gameObject.transform;
        _camera.transform.position = new Vector3(0, 0.8f, 0.6f) + gameObject.transform.position;
        _camera.GetComponent<MouseLook>().enabled = true;
        _camera.GetComponent<MouseLook>().SetPlayerTransform(transform);

        currentSpeed = walkSpeed;
    }

    public void Move(Vector3 movement)
    {
        currentSpeed = walkSpeed;

        characterController.Move(movement);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}