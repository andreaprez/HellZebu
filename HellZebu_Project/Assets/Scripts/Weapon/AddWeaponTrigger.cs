using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWeaponTrigger : MonoBehaviour
{
    private BoxCollider pickUpTrigger;
    private Weapon aRW;

    void Start()
    {
        pickUpTrigger = GetComponent<BoxCollider>();
        aRW = GetComponent<Weapon>();
        aRW.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            if (other.GetComponent<Controller>().weaponSlot1.empty || other.GetComponent<Controller>().weaponSlot2.empty)
            {
                other.GetComponent<Controller>().AddWeapon(this.gameObject);
                Destroy(pickUpTrigger);
                aRW.enabled = true;
                Destroy(this);
            }

        }
    }
}
