using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSpecialShootTrigger : MonoBehaviour
{
    public Transform playerPos;
    public ShotgunWeapon shotgunWeapon;

    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);

        if (other.tag.Contains("Enemy"))
        {
            if (other.transform.parent.GetComponent<Rigidbody>() != null)
            {
                Vector3 dir = other.transform.position - playerPos.position;
                dir.Normalize();
                other.transform.parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.transform.parent.GetComponent<Rigidbody>().AddForce(dir * shotgunWeapon.specialShotKnockBackForce);
                
            }

            if (other.transform.transform.parent.tag.Contains("Enemy"))
            {
                other.transform.parent.SendMessage("Damage");
            }
            else other.transform.SendMessage("Damage");
            
            
        }
    }
}
