using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookPlayer : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = player.transform.position - this.transform.position;
        transform.forward = -dir.normalized;
    }
}
