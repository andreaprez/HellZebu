using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float timeToDestroy;
    void Update()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
}
