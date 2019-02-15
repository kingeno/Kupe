using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCube : MonoBehaviour
{
    public float xRotation, yRotation, zRotation;
    public float rotationSpeed;

    public float movingSpeed;

    private void Update()
    {
        transform.Rotate(Random.Range(5f, 10f) * (rotationSpeed * Time.deltaTime), Random.Range(5f, 10f) * (rotationSpeed * Time.deltaTime), Random.Range(5f, 10f) * (rotationSpeed * Time.deltaTime));

        transform.Translate(Vector3.down * (movingSpeed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BottomBox")
            transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(20f, 23f));
    }
}
