using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssaultRifleWeapon : Weapon
{
   
    [Header("AssaultRifle")]
    public float maxSpreadValue;
    public float minSpreadValue;
    public float spreadShotCost;
    public float reduceSpreadSpeed;
    public float spreadCrossfireModifier;
    public Vector2 crossfireMinSize;
    public float specialShotCost;
    public float specialShotRayDuration;
    public bool useRecoil;
    public float recoilAmountX;
    public float recoilAmountY;
    public float recoilTime;
    public bool recoilDown;
    private RectTransform spreadCrossfire;
    private float currentSpreadValue;
    public Animation rifleAnimation;
    public AnimationClip shootClip;
    [Header("Draw gizmos")]
    public bool enableGizmos;

    //Shooting direction
    private float rayDistance=90000f;
    private Vector3 cameraRayCastDir;
    private Vector3 bulletDirection;
    private Quaternion rotationDirection;


    // Start is called before the first frame update
    public override void Start()
    {
        spreadCrossfire = MainCanvas.Instance.spreadCrossFire;
        currentSpreadValue = minSpreadValue;
                    
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (currentWeaponSlot.active)
        {
            spreadCrossfire.sizeDelta = crossfireMinSize + (new Vector2(currentSpreadValue, currentSpreadValue) * spreadCrossfireModifier);
           // spreadCrossfire.sizeDelta = new Vector2(Mathf.Clamp(spreadCrossfire.sizeDelta.x, 50, 150), Mathf.Clamp(spreadCrossfire.sizeDelta.y, 50, 150));
        }
        if (currentSpreadValue > minSpreadValue)
        {
            currentSpreadValue -= reduceSpreadSpeed * Time.deltaTime;            
        }
       
        base.Update();
    }
    public override void Shoot()
    {
        //Antes o Despues
        if (useRecoil)
        {
            Controller.Instance.Recoil(recoilAmountY, recoilAmountX, recoilTime, recoilDown);
        }
        //Calculate spread direction
        cameraRayCastDir = Camera.main.transform.forward;
        rotationDirection = Quaternion.Euler(Random.Range(-currentSpreadValue/10, currentSpreadValue/10), Random.Range(-currentSpreadValue/10, currentSpreadValue/10), 0);
        cameraRayCastDir = rotationDirection * cameraRayCastDir;
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, cameraRayCastDir, out hit, rayDistance, ignoreMasks);
        
        
        //Calculate direction from shooting point to hit
        bulletDirection = (hit.point - shootingPoint.transform.position);
        bulletDirection.Normalize();
                             
        
        //Create projectile and set direction
        GameObject b = Instantiate(currentAmmo, shootingPoint.transform.position, rotationDirection);

        if (currentAmmo == fireAmmo)
        {
            b.transform.parent = GameObject.Find("FireHiddenObjects").transform;
            GameObject muzz = Instantiate(muzzlePrefFireRifle, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
            muzz.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            b.transform.parent = GameObject.Find("IceHiddenObjects").transform;
            GameObject muzz = Instantiate(muzzlePrefIceRifle, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
            muzz.transform.localEulerAngles = Vector3.zero;
        }

        Projectile bScript = b.GetComponent<Projectile>();
        bScript.projectileDirection = bulletDirection;
        bScript.normalHit = hit.normal;
        bScript.hitPoint = hit.point;
     
        base.Shoot();
        FMODUnity.RuntimeManager.PlayOneShot(Shot, transform.position);
      
        rifleAnimation.CrossFade(shootClip.name, 0f, PlayMode.StopAll);
                

        currentSpreadValue += spreadShotCost;
        if (currentSpreadValue > maxSpreadValue)
        {
            currentSpreadValue = maxSpreadValue;
        }
      

    }
    public override void WChangeShoot()
    {
     
        base.WChangeShoot();
    }
    public override void SpecialShoot()
    {
        if (weaponElementalMode == WeaponElementalModes.Fire)
        {
            currentOverheatValueFire += specialShotCost;

        }
        else
        {
            currentOverheatValueIce += specialShotCost;
        }
        //Calculate direction
        cameraRayCastDir = Camera.main.transform.forward;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.transform.position, cameraRayCastDir, rayDistance, ignoreMasks);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag.Contains("Enemy"))
            {
                if (hit.transform.transform.parent.tag.Contains("Enemy"))
                    hit.transform.parent.SendMessage("Damage");
                else hit.transform.SendMessage("Damage");
            }
        }



        //Calculate direction from shooting point to hit
        bulletDirection = (hits[hits.Length - 1].point - shootingPoint.transform.position);
        bulletDirection.Normalize();
        lr0.SetPosition(0, rayShootingPoint.transform.position);
        lr0.SetPosition(1, hits[hits.Length - 1].point);
        lr1.SetPosition(0, rayShootingPoint.transform.position);
        lr1.SetPosition(1, hits[hits.Length - 1].point);

        lr0.startColor = Color.red;
        lr0.endColor = Color.red;
        lr1.startColor = Color.red;
        lr1.endColor = Color.red;
        StartCoroutine(activateLineRenderer(specialShotRayDuration));
        FMODUnity.RuntimeManager.PlayOneShot(specialShotRifle, transform.position);

        base.SpecialShoot();
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (enableGizmos)
            {
                //CameraForward
                Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * rayDistance));
                Gizmos.color = Color.red;
                //Shooting point to camera raycast hit point
                Gizmos.DrawLine(shootingPoint.transform.position, shootingPoint.transform.position + (bulletDirection * rayDistance));
                Gizmos.color = Color.blue;
                //Camera forward with spread applied;
                Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (cameraRayCastDir * rayDistance));

            }
        }
    }
}
