using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveParticle : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem bats;
    void Start()
    {
        if (!bats)
        {
            bats = GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!bats.isPlaying)
            {
                bats.Play();

            }
        }
       
    }
}
