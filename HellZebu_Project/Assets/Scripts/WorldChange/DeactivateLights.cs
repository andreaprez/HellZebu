using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLights : MonoBehaviour
{
    public GameObject particles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("HideFromCamera"))
        {
            //particles.layer = LayerMask.NameToLayer("HideFromCamera");
        }
        else
        {
            if (particles != null)
            {
                foreach (Transform t in particles.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = LayerMask.NameToLayer("AlwaysVisible");
                }
            }
        }
    }
}
