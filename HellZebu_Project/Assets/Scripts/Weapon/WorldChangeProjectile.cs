using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChangeProjectile : Projectile
{
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("DynamicWorld"))
        {
            
            WorldChangerManager.Instance.CollisionFilterAndCullingMaskWorldTargetChange(other.gameObject);
            Destroy(this.gameObject);

        }
        if (other.tag.Contains("Enemy"))
        {
            other.transform.parent.GetComponent<Enemy>().ChangeWorld();
        }
        base.OnTriggerEnter(other);
    }

}
