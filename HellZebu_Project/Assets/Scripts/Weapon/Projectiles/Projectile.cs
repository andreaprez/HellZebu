using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public Vector3 projectileDirection;
    public float projectileSpeed;
    private Rigidbody projectileRigidBody;
    public Vector3 normalHit;
    public Vector3 hitPoint;

    public GameObject hitParticle;
    public GameObject testingDecal;
    
    public float noOverlapValue;




    public virtual void Start()
    {

        projectileRigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        projectileRigidBody.velocity = projectileDirection.normalized * projectileSpeed * Time.deltaTime;

        

    }
    public virtual void Update()
    {
        
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SpawnerActivator")
        {
            other.GetComponent<SpawnerActivation>().Activate();
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("InvisibleTrigger") && !other.gameObject.CompareTag("Projectile") && !other.gameObject.CompareTag("Centipede") && other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Vector3 position = hitPoint;
            GameObject decal = Instantiate(testingDecal, hitPoint + normalHit * noOverlapValue, Quaternion.identity);
            decal.transform.parent = other.gameObject.transform;
            decal.transform.forward = normalHit;

            GameObject hitP = Instantiate(hitParticle, hitPoint + normalHit * noOverlapValue, Quaternion.identity);
            hitP.transform.parent = other.gameObject.transform;
            hitP.transform.forward = normalHit;

        }
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SpawnerActivator")
        {
            collision.gameObject.GetComponent<SpawnerActivation>().Activate();
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("InvisibleTrigger") && !collision.gameObject.CompareTag("Projectile") && !collision.gameObject.CompareTag("Centipede") && collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Vector3 position = hitPoint;
            GameObject decal = Instantiate(testingDecal, hitPoint + normalHit * noOverlapValue, Quaternion.identity);
            decal.transform.parent = collision.gameObject.transform;
            decal.transform.forward = normalHit;

            GameObject hitP = Instantiate(hitParticle, hitPoint + normalHit * noOverlapValue, Quaternion.identity);
            hitP.transform.parent = collision.gameObject.transform;
            hitP.transform.forward = normalHit;
        }
        Destroy(this.gameObject);
    }
}
