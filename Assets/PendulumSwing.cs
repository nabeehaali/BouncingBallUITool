using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    public float amplitude = 45f; // Maximum angle the pendulum swings
    public float frequency = 1f;   // Speed of the pendulum swing

    private float angle, lastAngle, angleOffset;
    private Vector3 rotationAxis;

    int counter;

    void Start()
    {
        // Initialize the pendulum's rotation axis
        rotationAxis = transform.forward;
        lastAngle = angle;
        counter = 0;
        angleOffset = 0;
    }

    void Update()
    {
        /*
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        float speed = 4;

        transform.position += new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
        */
        
        // Calculate the pendulum's rotation angle using sine function
        angle = amplitude * Mathf.Sin(frequency * Time.time); //+angleOffset if accounting for collisions
        //Time.time is good here because it is a continuous process, rather than smoothing in one frame

        // Rotate the pendulum
        transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);

        //change amplitude damping everytime it goes through a whole cycle
        float currAngle = angle;
        if(Mathf.Sign(lastAngle) != Mathf.Sign(currAngle))
        {
            counter++;
            if(counter % 2 == 0)
            {
                amplitude -= 5; //could publicize this

                if (amplitude <= 0)
                {
                    amplitude = 0;
                }
            }
        }
        lastAngle = currAngle;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Walls")
        {
            //make that the max amplitude
            //amplitude = transform.rotation.z * 100;
            frequency *= -1;
            //amplitude *= 0.8f;

            angleOffset = angle - amplitude * Mathf.Sin(frequency * Time.time);
        }
    }
    //if the ball collides with the wall, reverse the direction of the movement (angle *= -1)
}
