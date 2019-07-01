using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float movementSpeed = 50f;
    [SerializeField] float yawSpeed = 50f; //left and right rotatiom
    [SerializeField] float pitchSpeed = 50f; //up and down rotation
    [SerializeField] float rollSpeed = 50f; //roll rotation

    Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        Thrust();
        Turn();
    }

    void Turn()
    {
        float yaw = yawSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        float pitch = pitchSpeed * Time.deltaTime * Input.GetAxis("Pitch");
        float roll = rollSpeed * Time.deltaTime * Input.GetAxis("Roll");
        myTransform.Rotate(pitch, yaw, roll);
    }

    void Thrust()
    {
        myTransform.position += myTransform.forward * movementSpeed * Time.deltaTime * Input.GetAxis("Vertical");
    }
}
