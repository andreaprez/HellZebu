﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nightcoreTrigger : MonoBehaviour
{
    // Start is called before the first frame 


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            SceneManager.LoadScene(10);
        }
    }
}