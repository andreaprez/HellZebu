using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerActivation : MonoBehaviour
{
    SpawnManager thisSpawner;
    MeshRenderer mR;
    public GameObject text;
    Collider sCollider;
    public GameObject[] hpPositions;
    public GameObject hpPrefab;
    public GameObject otherSpawner;

    void Start()
    {
      //  mR = GetComponent<MeshRenderer>();
        thisSpawner = GetComponent<SpawnManager>();
        sCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activate()
    {
        otherSpawner.SetActive(false);
        for (int i = 0; i < hpPositions.Length; i++)
        {
            hpPositions[i].SetActive(true);
        }

        Destroy(sCollider);
        Destroy(text);

        thisSpawner.start = true;
       // mR.enabled = false;

        Destroy(this);
    }
   
}
