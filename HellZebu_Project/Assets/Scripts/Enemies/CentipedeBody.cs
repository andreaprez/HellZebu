using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBody : MonoBehaviour
{
    private Centipede centipede;

    private void Start()
    {
        centipede = transform.parent.GetComponent<Centipede>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && EnemyGlobalBlackboard.playerController.Vulnerable)
        {
            centipede.PlayerCollision(other.GetComponent<Controller>());
        }
    }

    private void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer("Enemies");
            }
        }
    }
}
