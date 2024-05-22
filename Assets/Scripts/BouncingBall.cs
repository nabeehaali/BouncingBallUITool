using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BouncingBall : MonoBehaviour
{
    //Mouse information (for picking up and dragging)
    private Vector2 mouseDifference = Vector2.zero;

    //Basic physics
    private float gravity = 9.8f;
    private Vector2 velocity;
    private Vector2 lastVelocity;
    private float initialHeight;
    private float endHeight;

    private bool isGrounded = false; //Ball is not bouncing
    private bool isDragged = false; //Mouse is not dragging an object

    //Projectile motion information
    private Vector2 projectileStartPos = Vector2.zero;
    private Vector2 projectileEndPos = Vector2.zero;

    //Information for duration of click/hold with the mouse
    private float clickStartTime;
    private float clickEndTime;

    [SerializeField]
    public float throwThreshold; //Time value to differentiate a drag versus throw (how long the mouse has to hold the ball before considering it being dragged)
    public int projectileForce; //Amount of force the ball has when it's being thrown
    public Vector2 startingVelocity; //How the ball moves on start
    public Vector2 squashScale; //Scale transformation on the object to show a 'squash' effect
    public Vector2 stretchScale; //Scale transformation on the object to show a 'stretch' effect
    private float heightThreshold = 0.3f;

    void Start()
    {
        velocity = startingVelocity;
        lastVelocity = velocity;
    }

    void Update()
    {
        if (isDragged)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(
                Mathf.Clamp(mousePos.x, -4, 4),
                Mathf.Clamp(mousePos.y, -4, 4)
            );
        }
        //When the mouse is not interacting with the object
        if (!isDragged)
        {
            //Move object downwards with acceleration based on gravity
            velocity.y -= gravity * Time.deltaTime;

            //calcualte height to determine when the ball has reached its peak (helpful to calculate the height displacement to determine when to stop the ball)
            Vector2 currentVel = velocity;
            if(Mathf.Sign(lastVelocity.y) > 0 && Mathf.Sign(currentVel.y) < 0)
            {
                initialHeight = transform.position.y;
            }
            lastVelocity = currentVel;

            //If the ball is no longer bouncing
            if (isGrounded)
            {
                velocity = Vector2.zero;
                transform.localScale = new Vector3(1, 1, 1);
            }

            //Adjust the position of the ball
            transform.position += (Vector3)velocity * Time.deltaTime;

            //Clamping
            CheckBounds();

            //Adjust the rotation of the ball
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Change vertical motion of the ball if it hits the top or bottom
        if (collision.gameObject.CompareTag("Ceiling") || collision.gameObject.CompareTag("Floor"))
        {
            velocity.y *= -0.9f;
            transform.localScale = new Vector3(squashScale.x, squashScale.y, 1);
            //When the ball hits the floor, count the number of bounces to determine when it should stop in place
            if (collision.gameObject.CompareTag("Floor"))
            {
                endHeight = transform.position.y;
                if(initialHeight - endHeight < heightThreshold)
                {
                    isGrounded = true;
                }
            }
        }
        //Change horizontal motion of the ball if it hits the right or left sides
        if (collision.gameObject.CompareTag("Walls"))
        {
            velocity.x *= -0.9f;
            transform.localScale = new Vector3(stretchScale.x, stretchScale.y, 1);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        //Reset the transformations of the ball when it is not colliding with any of the walls
        if (collision.gameObject.CompareTag("Ceiling") || collision.gameObject.CompareTag("Floor"))
        {
            transform.localScale = new Vector3(1, 1, 1);
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnMouseDown()
    {
        isDragged = true; //Avoid any physics based movement when picking up an object

        clickStartTime = Time.time; //Get time step from when object is picked up
        mouseDifference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position; //Difference between the mouse position and the object's position
        projectileStartPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get intial point of where to start the projectile motion
    }

    private void OnMouseDrag()
    {
        //Ball follows the mouse position from the point it gets picked up
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos - mouseDifference;
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnMouseUp()
    {
        isDragged = false; //Activates physics on the ball
        clickEndTime = Time.time; //Get time step from when the object is let go
        projectileEndPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get end point of where the projectile will land

        //Amount of time the mouse was held on the ball
        float clickDuration = clickEndTime - clickStartTime;

        // Calculate the direction and magnitude of the launch
        Vector2 launchDirection = projectileEndPos - projectileStartPos;
        launchDirection.Normalize();

        // Calculate the initial velocity components with exaggeration (based on projectile force)
        float initialVelocityX = launchDirection.x * clickDuration * projectileForce;
        float initialVelocityY = launchDirection.y * clickDuration * projectileForce;

        //Check whether the mouse interaction was a drag or throw (based on how long the ball was held for)
        if (clickDuration < throwThreshold && throwThreshold > 0)
        {
            velocity = new Vector2(initialVelocityX, initialVelocityY);
        }
        else
        {
            velocity = Vector2.zero;
        }

        // Make sure the ball position remains within the bounds
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -4, 4),
            Mathf.Clamp(transform.position.y, -4, 4)
        );
    }

    private void CheckBounds()
    {
        // Check and handle boundaries for x
        if (transform.position.x > 4)
        {
            transform.position = new Vector2(4, transform.position.y);
            velocity.x = -Mathf.Abs(velocity.x); // Reverse direction to simulate bounce
        }
        else if (transform.position.x < -4)
        {
            transform.position = new Vector2(-4, transform.position.y);
            velocity.x = Mathf.Abs(velocity.x); // Reverse direction to simulate bounce
        }

        // Check and handle boundaries for y
        if (transform.position.y > 4)
        {
            transform.position = new Vector2(transform.position.x, 4);
            velocity.y = -Mathf.Abs(velocity.y); // Reverse direction to simulate bounce
        }
        else if (transform.position.y < -4)
        {
            transform.position = new Vector2(transform.position.x, -4);
            velocity.y = Mathf.Abs(velocity.y); // Reverse direction to simulate bounce
        }
    }

    public void AssignRandomVel()
    {
        startingVelocity = new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(0, 5));
    }

    public void SetDefaultVel()
    {
        startingVelocity = new Vector2(0, 5);
    }

    public void ShowBallSquash()
    {
        transform.localScale = new Vector3(squashScale.x, squashScale.y, 1);
    }

    public void ShowBallStretch()
    {
        transform.localScale = new Vector3(stretchScale.x, stretchScale.y, 1);
    }

    public void ResetScales()
    {
        transform.localScale = Vector3.one;
    }
}
