using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CubeRotation : MonoBehaviour {

    // script blog post origin : https://blog.susfour.net/?p=197
    // to rotate a parallepiped : https://blog.susfour.net/memo/unity-script-for-roll-a-rectangular-parallelpiped/

    public float rotationPeriod = 0.3f;     // Temps nécessaire pour passer à côté de
    public float sideLength = 1f;           // Cube La longueur du côté de


    bool isRotate = false;                  // Cube Est en rotation ou pas
    float directionX = 0;                   // Indicateur de direction de rotation
    float directionZ = 0;                   // Indicateur de direction de rotation

    Vector3 startPos;                       // Avant rotation Cube Position
    float rotationTime = 0;                 // Temps qui passe pendant la rotation
    float radius;                           // Rayon de centrage orbital
    Quaternion fromRotation;                // Quotanion de Cube avant rotation
    Quaternion toRotation;                  // Quotanion de Cube après rotation

    void Start()
    {
        // Calculer le rayon du centre de gravité de l'orbite en rotation
        radius = sideLength * Mathf.Sqrt(2f) / 2f;
    }


    void Update()
    {
        float x = 0;
        float y = 0;

        // Ramassez l'entrée clé.
        x = Input.GetAxisRaw("Horizontal");
        if (x == 0)
        {
            y = Input.GetAxisRaw("Vertical");
        }


        // S'il y a une entrée clé et que le cube ne tourne pas, faites-le pivoter.
        if ((x != 0 || y != 0) && !isRotate)
        {
            directionX = y;                                                             // Sens de rotation défini (x ou y doivent être 0)
            directionZ = x;                                                             // Sens de rotation défini (x ou y doivent être 0)
            startPos = transform.position;                                              // Maintenir les coordonnées avant la rotation
            fromRotation = transform.rotation;                                          // Garder le quaternion avant la rotation
            transform.Rotate(directionZ * 90, 0, directionX * 90, Space.World);     // Rotation de 90 degrés dans le sens de la rotation
            toRotation = transform.rotation;                                            // Conserver le quaternion après la rotation
            transform.rotation = fromRotation;                                          // Retourner la rotation du cube avant la rotation. (N'est-ce pas une copie superficielle de Transformer ou ...?)
            rotationTime = 0;                                                           // Définissez le temps écoulé pendant la rotation sur 0.
            isRotate = true;                                                            // Définir un drapeau rotatif.
        }
    }

    void FixedUpdate()
    {

        if (isRotate)
        {

            rotationTime += Time.fixedDeltaTime;                                    // Augmenter le temps écoulé
            float ratio = Mathf.Lerp(0, 1, rotationTime / rotationPeriod);          // Pourcentage du temps écoulé actuel par rapport au temps de rotation

            // Déplacer
            float thetaRad = Mathf.Lerp(0, Mathf.PI / 2f, ratio);                   // Angle de rotation en radians.
            float distanceX = -directionX * radius * (Mathf.Cos(45f * Mathf.Deg2Rad) - Mathf.Cos(45f * Mathf.Deg2Rad + thetaRad));      // Distance parcourue sur l'axe X. Le signe de - sert à aligner la direction du mouvement sur la clé.
            float distanceY = radius * (Mathf.Sin(45f * Mathf.Deg2Rad + thetaRad) - Mathf.Sin(45f * Mathf.Deg2Rad));                        // Distance de déplacement de l'axe Y
            float distanceZ = directionZ * radius * (Mathf.Cos(45f * Mathf.Deg2Rad) - Mathf.Cos(45f * Mathf.Deg2Rad + thetaRad));           // Distance de déplacement de l'axe Z
            transform.position = new Vector3(startPos.x + distanceX, startPos.y + distanceY, startPos.z + distanceZ);                       // Définir la position actuelle

            // Rotation
            transform.rotation = Quaternion.Lerp(fromRotation, toRotation, ratio);      // Définir l’angle de rotation actuel avec Quaternion.Lerp (quelle fonction utile)

            // Initialise chaque paramètre en fin de mouvement / rotation. Abaissez le drapeau isRotate.
            if (ratio == 1)
            {
                isRotate = false;
                directionX = 0;
                directionZ = 0;
                rotationTime = 0;
            }
        }
    }
}
