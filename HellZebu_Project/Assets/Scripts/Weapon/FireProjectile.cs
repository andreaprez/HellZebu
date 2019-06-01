﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : Projectile
{

    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }
    
    public override void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            if (other.gameObject.transform.parent.CompareTag("Enemy"))
                other.gameObject.transform.parent.SendMessage("Damage");
            else other.gameObject.SendMessage("Damage");
        }
        base.OnTriggerEnter(other);
    }
}
