using UnityEngine;

[System.Serializable]
public class Slide
{
    public float slideSpeed = 5f;       /// speed
    public int degree = 0;              /// direction in degrees (0 = right, 90 = up, 180 = left, 270 = down)
    public float duration = 1f;         /// Duration of the slide in seconds
    public bool resetPosition = true; /// Whether to reset the position after sliding
    public bool IsSliding => isSliding; /// Property to check if the object is currently sliding

    [HideInInspector] private float timeRemaining = 0f;   ///Time remaining for the slide
    [HideInInspector] private bool isSliding = false; 
    [HideInInspector] private Vector3 slideDirection;     /// Direction of the slide
    [HideInInspector] private Vector3 initialPosition; /// Initial position of the object
    public void StartAnimation(Transform target)
    {
        slideDirection = new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0).normalized;
        /** 
        Convert the 'degree' value (in degrees) into a direction vector.
        Unity's trigonometric functions use radians, so we convert degrees to radians.
        Cosine gives the X (horizontal) component of the direction.
        Sine gives the Y (vertical) component of the direction.
        This creates a 2D vector pointing in the specified angle on the XY plane.
        We normalize the vector to ensure its length is always 1,
        so that the slide speed remains consistent regardless of the angle.
        */

        initialPosition = target.position; /// Then store the initial position of the object and activate the flag to start animation
        timeRemaining = duration;
        isSliding = true;
    }

    public void UpdateAnimation(Transform transform)
    {
        /** This method updates the position of the object during the slide animation.
        It moves the object in the specified direction at the defined speed for the duration of the slide
        */
        if (!isSliding) return;

        transform.position += slideDirection * slideSpeed * Time.deltaTime;
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            isSliding = false;
            if (resetPosition)
            {
                transform.position = initialPosition;
            }
        }
    }
}
