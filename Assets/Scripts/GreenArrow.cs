using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenArrow : MonoBehaviour {

    public Quaternion forwardArrow;
    public Quaternion backArrow;
    public Quaternion leftArrow;
    public Quaternion rightArrow;

    public bool isActive;

    public bool canBeRotated;

    void Start()
    {
        canBeRotated = true;

        forwardArrow = Quaternion.Euler(0, 0, 0);
        backArrow = Quaternion.Euler(0, 180, 0);
        leftArrow = Quaternion.Euler(0, 270, 0);
        rightArrow = Quaternion.Euler(0, 90, 0);

        
        if (transform.rotation == forwardArrow)
        {
            gameObject.tag = "Forward Arrow";
        }
        if (transform.rotation == rightArrow)
        {
            gameObject.tag = "Right Arrow";
        }
        if (transform.rotation == backArrow)
        {
            gameObject.tag = "Back Arrow";
        }
        if (transform.rotation == leftArrow)
        {
            gameObject.tag = "Left Arrow";
        }
    }

    private void OnMouseOver()
    {
        if (canBeRotated && Input.GetKeyDown(KeyCode.R))
        {
            if (transform.rotation == forwardArrow)
            {
                transform.rotation = rightArrow;
                gameObject.tag = "Right Arrow";
            }
            else if (transform.rotation == rightArrow)
            {
                transform.rotation = backArrow;
                gameObject.tag = "Back Arrow";
            }
            else if (transform.rotation == backArrow)
            {
                transform.rotation = leftArrow;
                gameObject.tag = "Left Arrow";
            }
            else if (transform.rotation == leftArrow)
            {
                transform.rotation = forwardArrow;
                gameObject.tag = "Forward Arrow";
            }
        }
    }
}
