using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportConflictZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            other.GetComponent<Controller>().onConflictZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Player")) { other.GetComponent<Controller>().onConflictZone = false; }
    }
}
