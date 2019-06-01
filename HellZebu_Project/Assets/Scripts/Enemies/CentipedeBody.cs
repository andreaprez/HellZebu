using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBody : MonoBehaviour
{
    private Centipede centipede;
    private void Start() {
        centipede = transform.parent.GetComponent<Centipede>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            centipede.PlayerCollision(other.GetComponent<Controller>());
        }
    }
}
