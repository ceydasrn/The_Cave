using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyrEffDestroyer : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject, 3f);
    }
}
