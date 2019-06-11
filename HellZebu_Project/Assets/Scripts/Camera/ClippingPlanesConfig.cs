using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlanesConfig : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    void Start()
    {
        float[] distances = new float[32];
        distances[0] = 66;
        distances[1] = 66;
        distances[2] = 66;
        distances[3] = 66;
        distances[4] = 66;
        distances[5] = 66;
        distances[6] = 66;
        distances[7] = 66;
        distances[8] = 66;
        distances[9] = 66;
        distances[10] = 66;
        distances[11] = 66;
        distances[12] = 160;
        mainCamera.layerCullDistances = distances;
    }

}
