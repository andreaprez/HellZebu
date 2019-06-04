using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    [SerializeField] private Centipede centipede;

    private void Start() {
        centipede.weakPoints.Add(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Projectile")) {
            MainCanvas.Instance.ShowHitmarker();
            centipede.weakPoints.Remove(gameObject);
            centipede.Damage();
            Destroy(gameObject);
        }
    }
}
