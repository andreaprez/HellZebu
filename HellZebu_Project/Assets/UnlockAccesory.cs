using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAccesory : MonoBehaviour
{
    public bool rifleAcc;
    public GameObject text;
    //Weapon[] weapons;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            text.SetActive(true);
            if (rifleAcc)
            {
                GameObject.FindObjectOfType<AssaultRifleWeapon>().specialShootUnlocked = true;
            }
            else
            {
                GameObject.FindObjectOfType<ShotgunWeapon>().specialShootUnlocked = true;

            }
            /*
            weapons = GameObject.FindObjectsOfType<Weapon>();
            foreach(Weapon w in weapons)
            {
                Debug.Log(w);
                w.specialShootUnlocked = true;
            }*/
            Destroy(this.gameObject);
        }
    }
}
