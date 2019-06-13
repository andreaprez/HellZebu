using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLights : MonoBehaviour
{
    public GameObject particles;
    public GameObject light;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("HideFromCamera"))
        {
            //particles.layer = LayerMask.NameToLayer("HideFromCamera");
            if (light != null) light.SetActive(false);
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
            if (light != null) light.SetActive(true);
        }
    }
}
