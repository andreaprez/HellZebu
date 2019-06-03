using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLights : MonoBehaviour
{
    public GameObject lights;
    public GameObject particles;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("HideFromCamera"))
        {
            lights.SetActive(false);
            //particles.layer = LayerMask.NameToLayer("HideFromCamera");
        }
        else
        {
            lights.SetActive(true);
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
