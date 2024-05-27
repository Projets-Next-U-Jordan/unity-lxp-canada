using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    void Update()
    {
        // Rotate the duck randomly wether X Y or Z each frame
        // int i = Random.Range(0, 3);
        // if (i == 0) transform.Rotate(Random.Range(0, 20), 0, 0);
        // if (i == 1) transform.Rotate(0, Random.Range(0, 20), 0);
        // if (i == 2) transform.Rotate(0, 0, Random.Range(0, 20));
        transform.Rotate(0, Random.Range(0, 20), 0);
    }
}
