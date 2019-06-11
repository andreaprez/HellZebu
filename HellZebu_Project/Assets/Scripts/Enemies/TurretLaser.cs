using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretLaser : MonoBehaviour
{
    [SerializeField] private float recalculateTime;
    [SerializeField] private LineRenderer line;
    [SerializeField] private CapsuleCollider collider;
    //[SerializeField] private LayerMask obstacles;
    //public GameObject Target { set { target = value; } }
    public float Range { set { range = value; } }
    public float Speed { set { speed = value; } }
    public Transform SpawnPoint{ set { spawnPoint= value; } }
    
    private float recalculateTimer;
    private float range;
    private float speed;
    private Transform spawnPoint;
    //private GameObject target;

    private void Start()
    {
        
    }

    void Update() {
        if (Vector3.SqrMagnitude(line.GetPosition(0) - line.GetPosition(line.positionCount - 1)) < range * range) {
            recalculateTimer += Time.deltaTime * speed;
            if (recalculateTimer >= recalculateTime) {
                recalculateTimer = 0f;
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, Vector3.zero + line.positionCount * Vector3.forward);
                
                collider.transform.position += spawnPoint.forward/2f;
                collider.height = (line.GetPosition(line.positionCount-1) - line.GetPosition(0)).magnitude;
            }
        }

// !!!!!!! MOVING LASER
        /*recalculateTimer += Time.deltaTime;
        
        if (recalculateTimer >= recalculateTime) {
            recalculateTimer = 0f;
            //recalculate direction
            direction = (target.transform.position - transform.position).normalized;
            // check obstacles
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 4f, obstacles)) {
                if (target.transform.position.y > transform.position.y) direction = transform.up;
                else {
                    Ray rightRay = new Ray(transform.position, transform.right);
                    Ray leftRay = new Ray(transform.position, -transform.right);
                    if (Physics.Raycast(rightRay, out hit, 4f, obstacles))
                    {
                        if (Physics.Raycast(leftRay, out hit, 4f, obstacles))
                            direction = -transform.forward;
                        else direction = -transform.right;
                    }
                    else direction = transform.right;
                }
            }
        }
        // update rotation
        Quaternion directionRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, directionRot, Time.deltaTime * speed);
        base.Update();
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && EnemyGlobalBlackboard.playerController.Vulnerable) {
            other.SendMessage("TakeDamage","turretLaser");
            gameObject.SetActive(false);
        }
        else if (!other.gameObject.CompareTag("Enemy"))
            gameObject.SetActive(false);
    }

    public void Reset() {
        collider.transform.position = spawnPoint.position;
        collider.height = 0.1f;
        line.positionCount = 1;
        line.SetPosition(0, Vector3.zero);
    }
}
