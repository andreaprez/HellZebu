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
    public bool specialShootUnlocked;
    public float maxOverheatValue;
    public float shotCost;
    public float coolingSpeed;
    public float overheatedRecoveryTime;
    public GameObject shootingPoint, specialShotgunShootPoint;
    public LayerMask ignoreMasks;

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

    [Header("Ice")]
    public GameObject iceAmmo;
    public Material iceMaterial;
    public GameObject muzzlePrefIceRifle, muzzlePrefIceShot;


    public float currentOverheatValueIce;
    private bool overheatedIce;
    private float overheatLerpIce;

    [Header("WorldChange")]
    public GameObject accesory;
    public GameObject rayShootingPoint;
    public float rayDuration;
    public float timeBetweenShootsWC;
    public float timeBetweenShootsWCTimer;
    public LineRenderer lr;
    public Color rayColor;
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
    public Image fireHolder;
    public Color overheatedColor;
    public GameObject fireOverheatedText;
    public Image iceHolder;
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
    public string ChangeWeaponSound = "";


    private Controller playerController;

    void OnWorldChange()
    {
        ChangeWeaponState();
    }


    public virtual void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
        
        WorldChangerManager.worldChangeEvent += OnWorldChange;
        fireFillBar.fillAmount = iceFillBar.fillAmount = 0;
        fireUI.SetActive(true);
        iceUI.SetActive(false);
        overheatLerpFire = 0;
        overheatedFire = false;
        currentWeaponSlot = transform.parent.GetComponent<WeaponSlot>();

        currentAmmo = fireAmmo;
        lastMode = WeaponElementalModes.Fire;
        lr.startColor = rayColor;
        lr.endColor = rayColor;

        ChangeWeaponState();

        if (currentWeaponSlot.active) { ChangeWeapon(true); }
        else { ChangeWeapon(false); }
    }


    public virtual void Update()
    {
        if (Controller.Instance.pauseOn == false && !playerController.movementLocked)
        {
            //Testing UI
            fireFillBar.fillAmount = (float)currentOverheatValueFire / (float)maxOverheatValue;
            iceFillBar.fillAmount = (float)currentOverheatValueIce / (float)maxOverheatValue;

            if (!overheatedFire) {
                fireHolder.color = Color.white;
                fireFillBar.color = baseColorFireBar;
            }


            if (!overheatedIce) {
                iceHolder.color = Color.white;
                iceFillBar.color = baseColorIceBar;
            }

            if (currentWeaponSlot.active)
            {

                if (weaponShootMode == WeaponShootModes.Automatic)
                {
                    if ( Input.GetKey(InputsManager.Instance.currentInputs.shoot))
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
                                    fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    FMODUnity.RuntimeManager.PlayOneShot(OverHeated, transform.position);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;
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
                                    iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    FMODUnity.RuntimeManager.PlayOneShot(OverHeated, transform.position);
                                    currentOverheatValueIce = maxOverheatValue;
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
                                    fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;
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
                                    iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueIce = maxOverheatValue;
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
                                    fireHolder.color = overheatedColor;
                                    fireFillBar.color = overheatedColor;
                                    fireOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueFire = maxOverheatValue;
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
                                    iceHolder.color = overheatedColor;
                                    iceFillBar.color = overheatedColor;
                                    iceOverheatedText.SetActive(true);
                                    //clamp
                                    currentOverheatValueIce = maxOverheatValue;
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
            meshRenderer.enabled = true;
            accesory.SetActive(true);
            if (lastMode == WeaponElementalModes.Fire) { fireUI.SetActive(true); }
            else if (lastMode == WeaponElementalModes.Ice) { iceUI.SetActive(true); }
           // FMODUnity.RuntimeManager.PlayOneShot(ChangeWeaponSound, transform.position);
            weaponCanvas.SetActive(true);
            



        }
        else
        {
            accesory.SetActive(false);
            meshRenderer.enabled = false;
            fireUI.SetActive(false);
            iceUI.SetActive(false);
            weaponCanvas.SetActive(false);

        }

    }

    public void ActivateCanvas()
    {
        weaponCanvas.SetActive(true);
    }
    public virtual void Shoot()
    {

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

        lr.SetPosition(0, rayShootingPoint.transform.position);
        lr.SetPosition(1, hit.point);
        lr.startColor = rayColor;
        lr.endColor = rayColor;
        StartCoroutine(activateLineRenderer(rayDuration));
        if (hit.transform.tag.Contains("DynamicWorld"))
        {

            WorldChangerManager.Instance.CollisionFilterAndCullingMaskWorldTargetChange(hit.transform.gameObject);


        }
        if (hit.transform.tag.Contains("Enemy"))
        {
            hit.transform.GetComponent<Enemy>().ChangeWorld();
        }
        //SOUND
        FMODUnity.RuntimeManager.PlayOneShot(Teleport, transform.position);
    }
    public IEnumerator activateLineRenderer(float secs)
    {
        //lr.enabled = true;


        //for(int i = 0; i < (int)(60*secs)+1; i++)
        //{
        //    alphaLerpValue += i*Time.deltaTime;
        //    lr.material.color = Color.Lerp(Color.white, Color.clear, alphaLerpValue);
        //    yield return new WaitForSeconds(Time.deltaTime);
        //}
        //lr.enabled = false;
        //alphaLerpValue = 0;
        //yield  break;

        lr.enabled = true;

        while (alphaLerpValue < 1)
        {
            alphaLerpValue += Time.deltaTime / secs;
            lr.material.color = Color.Lerp(Color.white, Color.clear, alphaLerpValue);
            yield return new WaitForEndOfFrame();

        }
        lr.enabled = false;
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
            }
        }
    }

}
