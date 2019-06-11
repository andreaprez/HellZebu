using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateLights : MonoBehaviour
{
    public GameObject lights;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("HideFromCamera"))
        {
            lights.SetActive(false);
        }
        else lights.SetActive(true);
    }
}
