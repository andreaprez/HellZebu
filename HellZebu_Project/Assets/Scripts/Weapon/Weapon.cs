using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Weapon : MonoBehaviour
{


    public enum WeaponElementalModes { Fire, Ice, WorldChange }
    public enum WeaponShootModes { Automatic, Semiautomatic }
    public MeshRenderer meshRenderer;
    [Header("Weapon modes")]
    public WeaponShootModes weaponShootMode;
    public WeaponElementalModes weaponElementalMode = WeaponElementalModes.Fire;
    public float timeBetweenShootsAutomatic;
    public float timeBetweenShootsSemiutomatic;

    private float automaticWaitTimer;
    private float semiautomaticWaitTimer;


    [Header("Weapon general")]
    public Controller playerController;
    public bool specialShootUnlocked;
    public float maxOverheatValue;
    public float shotCost;
    public float coolingSpeed;
    public float overheatedRecoveryTime;
    public GameObject shootingPoint, specialShotgunShootPoint;
    public LayerMask ignoreMasks;
    public Material emissiveMaterial;
    public Texture emissiveTextureRed;
    public Texture emissiveTextureBlue;
    public GameObject emissiveEyes;
    public Light light;
    public Color redColor;
    public Color blueColor;
    public Color blueFlash;
    public Color redFlash;
    public Light shotFlash;
    public Animation weaponAnimation;
    public AnimationClip activateClip;
    public AnimationClip deactivateClip;

    protected GameObject currentAmmo;

    protected WeaponSlot currentWeaponSlot;

    [Header("Fire")]
    public GameObject fireAmmo;
    public Material fireMaterial;

    public float currentOverheatValueFire;
    private bool overheatedFire;
    private float overheatLerpFire;
    public GameObject muzzlePrefFireRifle, muzzlePrefFireShot;
    public GameObject specialShotgunPart;

    public GameObject overheatFireParticle;
    public GameObject overheatFireParticlePos;


    [Header("Ice")]
    public GameObject iceAmmo;
    public Material iceMaterial;
    public GameObject muzzlePrefIceRifle, muzzlePrefIceShot;


    public float currentOverheatValueIce;
    private bool overheatedIce;
    private float overheatLerpIce;


    public GameObject overheatIceParticle;
    public GameObject overheatIceParticlePos;

    [Header("WorldChange")]
    public GameObject accesory;
    public GameObject rayShootingPoint;
    public float rayDuration;
    public float timeBetweenShootsWC;
    public float timeBetweenShootsWCTimer;

    [Header("Lasers")]
    public GameObject laser;
    public GameObject laserSpecial;
    public LineRenderer lr0;
    public LineRenderer lr1;
    public Color rayColor, rayOutsideColor;
    public Color rayTransparentColor;
    public float alphaLerpValue;

    [Header("SpecialShootGeneral")]
    public float specialShotTime;
    public float specialShotTimer;

    [Header("TestingUI")]
    public GameObject weaponCanvas;
    public GameObject fireUI;
    public GameObject iceUI;
    public Image fireFillBar;
    public Image iceFillBar;
    public Color overheatedColor;
    public GameObject fireOverheatedText;
    public GameObject iceOverheatedText;
    public Color baseColorFireBar;
    public Color baseColorIceBar;

    /*public GameObject overheatFire;
    public GameObject overheatIce;*/

    public WeaponElementalModes lastMode;


    [Header("Sounds")]
    [FMODUnity.EventRef]
    public string OverHeated = "";

    [FMODUnity.EventRef]
    public string Shot = "";

    [FMODUnity.EventRef]
    public string Teleport = "";

 
    [FMODUnity.EventRef]
    public string specialShotRifle = "";
    [FMODUnity.EventRef]
    public string specialShotShotgun = "";

    void OnWorldChange()
    {
        ChangeWeaponState();
    }


    public virtual void Start()
    {
        WorldChangerManager.worldChangeEvent += OnWorldChange;
        fireFillBar.fillAmount = iceFillBar.fillAmount = 0;
        fireUI.SetActive(true);
        iceUI.SetActive(false);
        overheatLerpFire = 0;
        overheatedFire = false;
        currentWeaponSlot = transform.parent.GetComponent<WeaponSlot>();

        currentAmmo = fireAmmo;
        lastMode = WeaponElementalModes.Fire;
        lr0.startColor = rayColor;
        lr0.endColor = rayColor;
        lr1.startColor = rayColor;
        lr1.endColor = rayColor;

        ChangeWeaponState();

        if (currentWeaponSlot.active) { ChangeWeapon(true); }
        else { ChangeWeapon(false); }
    }


    public virtual void Update()
    {
        if (Controller.Instance.pauseOn == false)
        {
            //Testing UI
            fireFillBar.fillAmount = (float)currentOverheatValueFire / (float)maxOverheatValue;
            iceFillBar.fillAmount = (float)currentOverheatValueIce / (float)maxOverheatValue;

            if (!overheatedFire) {
                //fireHolder.color = Color.white;
                fireFillBar.color = baseColorFireBar;
            }


            if (!overheatedIce) {
                //iceHolder.color = Color.white;
                iceFillBar.color = baseColorIceBar;
            }

            if (currentWeaponSlot.active && !playerController.movementLocked)
            {

                if (weaponShootMode == WeaponShootModes.Automatic)
                {
                    if (Input.GetKey(InputsManager.Instance.currentInputs.shoot))
                    {
                        if (weaponElementalMode == WeaponElementalModes.Fire)
                        {
                            if (currentOverheatValueFire < maxOverheatValue && overheatedFire == false && automaticWaitTimer <= 0)
                            {
                                currentOverheatValueFire += shotCost;
                                Shoot();
                                if (currentOverheatValueFire >= maxOverheatValue)
                                {
                                    overheatedFire = true;
                                    //fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    FMODUnity.RuntimeManager.PlayOneShot(OverHeated, transform.position);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;

                                    GameObject overheatParticle = Instantiate(overheatFireParticle, overheatFireParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatFireParticlePos.transform;





                                }
                                automaticWaitTimer = timeBetweenShootsAutomatic;
                            }


                        }
                        if (weaponElementalMode == WeaponElementalModes.Ice)
                        {
                            if ((currentOverheatValueIce < maxOverheatValue) && overheatedIce == false && automaticWaitTimer <= 0)
                            {
                                currentOverheatValueIce += shotCost;
                                Shoot();
                                if (currentOverheatValueIce >= maxOverheatValue)
                                {
                                    overheatedIce = true;
                                    //iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    FMODUnity.RuntimeManager.PlayOneShot(OverHeated, transform.position);
                                    currentOverheatValueIce = maxOverheatValue;

                                    GameObject overheatParticle = Instantiate(overheatIceParticle, overheatIceParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatIceParticlePos.transform;
                                }
                                automaticWaitTimer = timeBetweenShootsAutomatic;
                            }
                        }


                    }


                    automaticWaitTimer -= Time.deltaTime;

                }
                else
                {
                    if (Input.GetKeyDown(InputsManager.Instance.currentInputs.shoot))
                    {
                        if (weaponElementalMode == WeaponElementalModes.Fire)
                        {
                            if ((currentOverheatValueFire < maxOverheatValue) && overheatedFire == false && semiautomaticWaitTimer <= 0)
                            {
                                currentOverheatValueFire += shotCost;
                                Shoot();
                                if (currentOverheatValueFire >= maxOverheatValue)
                                {
                                    overheatedFire = true;
                                    //fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;

                                    GameObject overheatParticle = Instantiate(overheatFireParticle, overheatFireParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatFireParticlePos.transform;
                                }
                                semiautomaticWaitTimer = timeBetweenShootsSemiutomatic;
                            }


                        }
                        if (weaponElementalMode == WeaponElementalModes.Ice)
                        {
                            if ((currentOverheatValueIce < maxOverheatValue) && overheatedIce == false && semiautomaticWaitTimer <= 0)
                            {
                                currentOverheatValueIce += shotCost;
                                Shoot();
                                if (currentOverheatValueIce >= maxOverheatValue)
                                {
                                    overheatedIce = true;
                                    //iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueIce = maxOverheatValue;

                                    GameObject overheatParticle = Instantiate(overheatIceParticle, overheatIceParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatIceParticlePos.transform;
                                }
                                semiautomaticWaitTimer = timeBetweenShootsSemiutomatic;
                            }
                        }

                    }
                    semiautomaticWaitTimer -= Time.deltaTime;
                }
                timeBetweenShootsWCTimer -= Time.deltaTime;
                if (Input.GetKeyDown(InputsManager.Instance.currentInputs.transferEnemy))
                {
                    if (timeBetweenShootsWCTimer <= 0)
                    {
                        if (MainCanvas.Instance.currentBullets > 0)
                        {
                            WChangeShoot();
                            timeBetweenShootsWCTimer = timeBetweenShootsAutomatic;
                            MainCanvas.Instance.currentBullets--;
                            MainCanvas.Instance.bulletRechargeTimer = MainCanvas.Instance.bulletRechargeTime;


                        }
                    }
                }
                specialShotTimer -= Time.deltaTime;
                if (specialShootUnlocked)
                {
                    if (Input.GetKeyDown(InputsManager.Instance.currentInputs.specialShoot))
                    {
                        if (weaponElementalMode == WeaponElementalModes.Fire)
                        {
                            if ((currentOverheatValueFire < maxOverheatValue) && overheatedFire == false && specialShotTimer <= 0)
                            {
                                //Sumamos el shotCost en la shotgun o rifle pq los valores son distintos.

                                SpecialShoot();
                                if (currentOverheatValueFire >= maxOverheatValue)
                                {
                                    overheatedFire = true;
                                    //fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;


                                    GameObject overheatParticle = Instantiate(overheatFireParticle, overheatFireParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatFireParticlePos.transform;

                                }
                                specialShotTimer = specialShotTime;
                            }
                        }
                        if (weaponElementalMode == WeaponElementalModes.Ice)
                        {
                            if ((currentOverheatValueIce < maxOverheatValue) && overheatedIce == false && specialShotTimer <= 0)
                            {
                                //Sumamos el shotCost en la shotgun o rifle pq los valores son distintos.

                                SpecialShoot();
                                if (currentOverheatValueIce >= maxOverheatValue)
                                {
                                    overheatedIce = true;
                                    //iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueIce = maxOverheatValue;

                                    GameObject overheatParticle = Instantiate(overheatIceParticle, overheatIceParticlePos.transform.position, Quaternion.identity);
                                    overheatParticle.transform.parent = overheatIceParticlePos.transform;
                                }
                                specialShotTimer = specialShotTime;
                            }
                        }
                    }

                }


            }

            if (overheatedFire)
            {
                //overheatFire.SetActive(true);
                overheatLerpFire = overheatLerpFire + (Time.deltaTime / overheatedRecoveryTime);
                currentOverheatValueFire = Mathf.Lerp(maxOverheatValue, 0, overheatLerpFire);
                if (overheatLerpFire >= 1)
                {
                    overheatedFire = false;
                    fireOverheatedText.SetActive(false);
                    overheatLerpFire = 0;
                    //clamp
                    currentOverheatValueFire = 0;
                }
            }
            if (overheatedIce)
            {
                //overheatIce.SetActive(true);
                overheatLerpIce = overheatLerpIce + (Time.deltaTime / overheatedRecoveryTime);
                currentOverheatValueIce = Mathf.Lerp(maxOverheatValue, 0, overheatLerpIce);
                if (overheatLerpIce >= 1)
                {
                    overheatedIce = false;
                    iceOverheatedText.SetActive(false);
                    overheatLerpIce = 0;
                    //clamp
                    currentOverheatValueIce = 0;
                }
            }


            /*if (overheatedFire == false) { overheatFire.SetActive(false); }
            if (overheatedIce == false) { overheatIce.SetActive(false); }*/

            if (overheatedFire == false && currentOverheatValueFire > 0)
            {

                currentOverheatValueFire -= coolingSpeed * Time.deltaTime;
                if (currentOverheatValueFire < 0) { currentOverheatValueFire = 0; }

            }

            if (overheatedIce == false && currentOverheatValueIce > 0)
            {
                //overheatIce.SetActive(false);
                currentOverheatValueIce -= coolingSpeed * Time.deltaTime;
                if (currentOverheatValueIce < 0) { currentOverheatValueIce = 0; }

            }


        }

    }
    public void ChangeWeapon(bool active)
    {
        if (active)
        {
            StartCoroutine("ActivateWeapon");
        }
        else
        {
            weaponAnimation.clip = deactivateClip;
            weaponAnimation.Play();
            
            StartCoroutine("DeactivateWeapon");
        }

    }

    public IEnumerator ActivateWeapon()
    {
        yield return new WaitForSeconds(0.1f);
        
        meshRenderer.enabled = true;
        if (emissiveEyes != null) emissiveEyes.SetActive(true);
        accesory.SetActive(true);
        if (lastMode == WeaponElementalModes.Fire) { fireUI.SetActive(true); }
        else if (lastMode == WeaponElementalModes.Ice) { iceUI.SetActive(true); }
         
        weaponCanvas.SetActive(true);
        light.gameObject.SetActive(true);
            
        weaponAnimation.clip = activateClip;
        weaponAnimation.Play();
    }

    public IEnumerator DeactivateWeapon()
    {
        yield return  new WaitForSeconds(0.15f);
        
        accesory.SetActive(false);
        meshRenderer.enabled = false;
        if (emissiveEyes != null) emissiveEyes.SetActive(false);
        fireUI.SetActive(false);
        iceUI.SetActive(false);
        weaponCanvas.SetActive(false);
        light.gameObject.SetActive(false);
    }

    public void ActivateCanvas()
    {
        weaponCanvas.SetActive(true);
    }
    public virtual void Shoot()
    {
        shotFlash.gameObject.SetActive(true);
        StartCoroutine("FlashTimer");
    }

    public IEnumerator FlashTimer()
    {
        yield return new WaitForSeconds(0.08f);
        shotFlash.gameObject.SetActive(false);
    }
    
    public virtual void WChangeShoot()
    {
        //Calculate direction
        Vector3 cameraRayCastDir;
        cameraRayCastDir = Camera.main.transform.forward;
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, cameraRayCastDir, out hit, 90000f, ignoreMasks);


        //Calculate direction from shooting point to hit
        Vector3 bulletDirection;
        bulletDirection = (hit.point - shootingPoint.transform.position);
        bulletDirection.Normalize();

        lr0.SetPosition(0, rayShootingPoint.transform.position - rayShootingPoint.transform.forward);
        lr0.SetPosition(1, hit.point);
        lr0.startColor = rayColor;
        lr0.endColor = rayColor;
        
        lr1.SetPosition (0, rayShootingPoint.transform.position - rayShootingPoint.transform.forward);
        lr1.SetPosition(1, hit.point);
        lr1.startColor = rayOutsideColor;
        lr1.endColor = rayOutsideColor;
        StartCoroutine(activateLineRenderer(rayDuration));
        if (hit.transform.tag.Contains("DynamicWorld"))
        {

            WorldChangerManager.Instance.CollisionFilterAndCullingMaskWorldTargetChange(hit.transform.gameObject);


        }
        if (hit.transform.tag =="Enemy")
        {
            hit.transform.GetComponent<Enemy>().ChangeWorld();
        }
     
        //SOUND
        FMODUnity.RuntimeManager.PlayOneShot(Teleport, transform.position);
    }
    public IEnumerator activateLineRenderer(float secs, bool special = false)
    {
        lr0.enabled = true;
        lr1.enabled = true;
        if (!special)
        {
            laser.SetActive(true);

            laser.transform.position = rayShootingPoint.transform.position + transform.forward * 10;
            laser.transform.up = transform.forward;
        }
        else
        {
            laserSpecial.SetActive(true);

            laserSpecial.transform.position = transform.position + transform.forward * 10;
            laserSpecial.transform.up = transform.forward;
        }
       

        while (alphaLerpValue < 1)
        {
            alphaLerpValue += Time.deltaTime / secs;
            lr0.material.color = Color.Lerp(Color.white, Color.clear, alphaLerpValue);
            lr1.material.color = Color.Lerp(Color.white, Color.clear, alphaLerpValue);
            yield return new WaitForEndOfFrame();

        }
        if(special)laserSpecial.SetActive(false);
        else laser.SetActive(false);

        lr0.enabled = false;
        lr1.enabled = false;
        alphaLerpValue = 0;
        yield break;

    }
    public virtual void SpecialShoot()
    {
        print("SpecialShoot");
    }

    public void ChangeWeaponState()
    {
        if (currentWeaponSlot != null)
        {
            if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Ice)
            {
                weaponElementalMode = WeaponElementalModes.Ice;
                currentAmmo = iceAmmo;
                meshRenderer.sharedMaterial = iceMaterial;
                lastMode = WeaponElementalModes.Ice;
                if (currentWeaponSlot.active)
                {
                    fireUI.SetActive(false);
                    iceUI.SetActive(true);
                }
                else
                {
                    fireUI.SetActive(false);
                    iceUI.SetActive(false);
                }

                emissiveMaterial.SetTexture("_EmissionMap", emissiveTextureBlue);
                light.color = blueColor;
                shotFlash.color = blueFlash;
            }
            else if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Fire)
            {
                weaponElementalMode = WeaponElementalModes.Fire;
                currentAmmo = fireAmmo;
                meshRenderer.sharedMaterial = fireMaterial;
                lastMode = WeaponElementalModes.Fire;
                if (currentWeaponSlot.active)
                {
                    fireUI.SetActive(true);
                    iceUI.SetActive(false);
                }
                else
                {
                    fireUI.SetActive(false);
                    iceUI.SetActive(false);
                }
                
                emissiveMaterial.SetTexture("_EmissionMap", emissiveTextureRed);
                light.color = redColor;
                shotFlash.color = redFlash;
            }
        }
    }

}
