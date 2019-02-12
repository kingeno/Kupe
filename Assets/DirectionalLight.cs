using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLight : MonoBehaviour
{
    public float xRotation;
    public float yRotation;
    public float zRotation;

    void Start()
    {
        transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);
    }
}
