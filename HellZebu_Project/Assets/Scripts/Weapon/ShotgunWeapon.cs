using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : Weapon
{
    [Header("Shotgun")]
    public int pellets;
    //not same spread as on AR
    public float spreadValue;
    public float spreadCrossfireModifier;
    private RectTransform spreadCrossfire;
    public float specialShotCost;
    public float specialShotKnockBackForce;
    public float specialShotJumpForce;
    private float specialShotDir;
    public float framesActives;
    public GameObject cone;
    public float recoilAmountX;
    public float recoilAmountY;
    public float recoilTime;
    public bool recoilDown;
    public Vector2 crossFireMinSize;
    public float minCrossfire;
    public float maxCrossfire;
    public float crossfireValue;
    public float reduceCrossfire;
    public float crossfireCost;
    public Animation shotgunAnimation;
    public AnimationClip shootClip;
    [Header("Draw gizmos")]
    public bool enableGizmos;

    //Shooting direction
    private float rayDistance=90000f;
    private Vector3 cameraRayCastDir;
    private Vector3 bulletDirection;
    private Quaternion rotationDirection;

    public GameObject muzzleSpecialPartIce, muzzleSpecialPartFire;

    public override void Start()
    {
        crossfireValue = minCrossfire;
        spreadCrossfire = MainCanvas.Instance.spreadCrossFire;
        base.Start();
    }
    public override void Update()
    {
        if (currentWeaponSlot.active)
        {
            spreadCrossfire.sizeDelta = crossFireMinSize + (new Vector2(crossfireValue, crossfireValue) * spreadCrossfireModifier);

        }
        if (crossfireValue > minCrossfire)
        {
            crossfireValue -= reduceCrossfire * Time.deltaTime;
        }
        
        base.Update();

    }
    public override void Shoot()
    {
        for (int i = 0; i < pellets; i++)
        {
            //Calculate spread direction
            cameraRayCastDir = Camera.main.transform.forward;
            rotationDirection = Quaternion.Euler(Random.Range(-spreadValue / 10, spreadValue / 10), Random.Range(-spreadValue / 10, spreadValue / 10), 0);
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
                GameObject muzz = Instantiate(muzzlePrefFireShot, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
                muzz.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                b.transform.parent = GameObject.Find("IceHiddenObjects").transform;
                GameObject muzz = Instantiate(muzzlePrefIceShot, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
                muzz.transform.localEulerAngles = Vector3.zero;
            }
            Projectile bScript = b.GetComponent<Projectile>();
            bScript.projectileDirection = bulletDirection;
            bScript.normalHit = hit.normal;
            bScript.hitPoint = hit.point;
            
           
        }
        crossfireValue += crossfireCost;
        base.Shoot();
        Controller.Instance.Recoil(recoilAmountY, recoilAmountX, recoilTime, recoilDown);
        FMODUnity.RuntimeManager.PlayOneShot(Shot, transform.position);
        shotgunAnimation.CrossFade(shootClip.name, 0f, PlayMode.StopAll);
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
            GameObject muzz = Instantiate(muzzleSpecialPartFire, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
            muzz.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            currentOverheatValueIce += specialShotCost;
            GameObject muzz = Instantiate(muzzleSpecialPartIce, shootingPoint.transform.position, Quaternion.identity, this.gameObject.transform);
            muzz.transform.localEulerAngles = Vector3.zero;
        }
        StartCoroutine(activateXFrames(framesActives));
        if(Vector3.Dot(Vector3.down, transform.forward) > 0.65f)
        {
            GameObject airWave = Instantiate(specialShotgunPart, specialShotgunShootPoint.transform.position, Quaternion.identity);
            airWave.transform.localEulerAngles = Vector3.zero;
          
            Controller.Instance.SpecialShotJump(-transform.forward, specialShotJumpForce);
        }
        FMODUnity.RuntimeManager.PlayOneShot(specialShotShotgun, transform.position);

        base.SpecialShoot();
    }
    IEnumerator activateXFrames(float frames)
    {
        cone.SetActive(true);
        yield return new WaitForSeconds(frames*Time.deltaTime);
        cone.SetActive(false);
        yield break;

    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (enableGizmos)
            {
                //CameraForward
                Gizmos.color = Color.magenta;
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
