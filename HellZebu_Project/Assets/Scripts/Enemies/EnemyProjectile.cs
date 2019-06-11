using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    protected float speed;
    protected Vector3 direction;
    public enum EWorld { ICE, FIRE };
    protected EWorld currentWorld;

    public float Speed { set {  speed = value; } }
    public Vector3 Direction { set { direction = value; } }
    public EWorld CurrentWorld { set { currentWorld = value; } }

    protected void Start() {
        if (currentWorld == EWorld.FIRE)
            transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
        else
            transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
    }

    protected void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            other.SendMessage("TakeDamage","skullProjectile");
        }
        if (!other.gameObject.CompareTag("Enemy")) {
            Destroy(gameObject);
        }
    }
}
