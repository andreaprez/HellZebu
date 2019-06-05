using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour, DataInterface
{
    public static Controller Instance;
    #region VARIABLES
    [Header("Camera Control")]
    [SerializeField] private bool invertedPitch = false;
    [SerializeField] private bool invertedYaw = false;
    [SerializeField] private float pitchRotationalSpeed;
    [SerializeField] private float yawRotationalSpeed;
    [SerializeField] private float minMouseSensitivity;
    [SerializeField] private float maxMouseSensitivity;
    [SerializeField] private float currentMouseSensitivity;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private Transform pitchController;
    [SerializeField] private float minFOV;
    [SerializeField] private float maxFOV;
    [SerializeField] private float currentFOV;
    [SerializeField] private Camera mainCamera;

    private float pitch;
    private float yaw;
    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTiltAngle;
    [SerializeField] private float movementTiltSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private bool  dashEnabled;
    [SerializeField] private float dashImpulse;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private float dashDuration;
    private float dashTimer = 0.0f;
    private bool dashCoolDownActive = false;
    private float dashCurrentSpeed;
    private float verticalSpeed;
    private bool onGround;
    private bool movementLocked;
    
    [Header("World Change")]
    [HideInInspector] public bool onConflictZone;

    [Header("Weapons")]
    public WeaponSlot weaponSlot1;
    public WeaponSlot weaponSlot2;
    public int activeWeapon;

    [Header("TestingUI")]
    public float worldChangeTime;

    public float worldChangeTimer;
    private PlayerData playerData;
    public Animator UIAnimator;

    [Header("Spawn")]
    //Da error al cargar escena porque ese transform se desvincula del controller al destruirse, lo hardcodeo de momento y ya veremos como lo hacemos
    private Transform respawnPoint;
    private bool respawning;
    [SerializeField] private CameraShake cameraShake;

    [Header("Health")]
    public int currentHealth = 5;
    [SerializeField] private float invulnerabilityTime = 3f; 
    private float invulnerabilityTimer;
    private bool vulnerable = true;
    [SerializeField] private ParticleSystem healParticle;

    public bool Vulnerable { get { return vulnerable; } }

    #endregion


    //Recoil
    public float lerpRecoil;
    public bool pauseOn;

    public void Recoil(float recoilAmountY, float recoilAmountX, float lerpTimeRecoil, bool recoilDown)
    {
        if (pauseOn == false)
        {


            StartCoroutine(AddRecoil(recoilAmountY / 100, recoilAmountX / 100, lerpTimeRecoil, recoilDown));
        }
    }
    IEnumerator AddRecoil(float recoilY, float recoilX, float lerpTimeRecoil, bool recoilDown)
    {
        if (pauseOn == false)
        {
            while (lerpRecoil <= 1)
            {

                lerpRecoil += Time.deltaTime / lerpTimeRecoil;
                pitch -= recoilY / (60 * lerpTimeRecoil);
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
                if (Random.Range(-1000, 1000) > 0)
                {
                    yaw += recoilX / (60 * lerpTimeRecoil);
                }
                else
                {
                    yaw -= recoilX / (60 * lerpTimeRecoil);
                }

                transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
                pitchController.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
                yield return new WaitForEndOfFrame();

            }
            lerpRecoil = 0;
            if (recoilDown)
            {
                while (lerpRecoil <= 0.5f)
                {

                    lerpRecoil += Time.deltaTime / lerpTimeRecoil;
                    pitch += recoilY / (60 * lerpTimeRecoil);
                    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                    transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
                    pitchController.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
                    yield return new WaitForEndOfFrame();

                }
                lerpRecoil = 0;
            }
        }
        yield break;


    }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;


        Instance = this;


    }


    private void Start()
    {
        MainCanvas.pauseOnEvent += OnPause;
        MainCanvas.pauseOffEvent += OffPause;
        DataManager.savingEvent += OnSave;
        DataManager.loadingEvent += OnLoad;

        playerData = new PlayerData();
        playerData = DataManager.gameData.playerData;
        worldChangeTimer = 0;
        MainCanvas.Instance.wChangeCD = worldChangeTimer;
        MainCanvas.Instance.maxWChangeCD = worldChangeTime;

        OffPause();
    }

    void Update()
    {
        currentMouseSensitivity = Mathf.Lerp(minMouseSensitivity, maxMouseSensitivity, OptionsManager.Instance.customOptions.mouseSensitivity);
        currentFOV = Mathf.Lerp(minFOV, maxFOV, OptionsManager.Instance.customOptions.FOV);
        mainCamera.fieldOfView = currentFOV;
        invertedYaw = OptionsManager.Instance.customOptions.invertXAxis;
        if (pauseOn == false)
        {
            MainCanvas.Instance.playerLife = currentHealth;

            if (!movementLocked)
            {
                ChangeActiveWeapon();
                Rotate();
                Move();


                if (worldChangeTimer > 0)
                {
                    worldChangeTimer -= Time.deltaTime;
                }

                if (worldChangeTimer <= 0)
                {
                    CheckWorldChange();
                }

                MainCanvas.Instance.wChangeCD = worldChangeTimer;

            }
        }
    }

    //Rotate camera (pitch + yaw) using mouse input
    void Rotate()
    {
        float mouseAxisY = Input.GetAxis("Mouse Y");
        float mouseAxisX = Input.GetAxis("Mouse X");

        if (invertedPitch) mouseAxisY = -mouseAxisY;
        pitch += mouseAxisY * pitchRotationalSpeed*currentMouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (invertedYaw) mouseAxisX = -mouseAxisX;
        yaw += mouseAxisX * yawRotationalSpeed * currentMouseSensitivity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
    }
    public void SpecialShotJump(Vector3 dir, float force)
    {
        verticalSpeed = force;

    }

    //Move player
    void Move()
    {
        Vector3 movement = Vector3.zero;

        //create normalized vector from inputs
        float movementAxisZ = Input.GetAxis("Vertical");
        float movementAxisX = Input.GetAxis("Horizontal");
        movement = (transform.forward * movementAxisZ + transform.right * movementAxisX);
        //apply tilt on roll axis
        pitchController.localRotation = Quaternion.Euler(pitchController.localRotation.eulerAngles.x, pitchController.localRotation.eulerAngles.y, -movementAxisX * movementTiltAngle * movementTiltSpeed);

        if (dashEnabled) Dash(ref movement); //check dash input
        Jump(); //check jump input

        movement.y = verticalSpeed;
        movement *= movementSpeed * Time.deltaTime;

        CollisionFlags collisionFlags = characterController.Move(movement);
        if ((collisionFlags & CollisionFlags.Below) != 0)
        {
            onGround = true;
            verticalSpeed = 0.0f;
        }
        else onGround = false;
        if ((collisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0.0f)
            verticalSpeed = 0.0f;

        verticalSpeed += gravity * Time.deltaTime;
        
        // check onGround (para poder saltar en bajada)
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2.2f))
            onGround = true;
    }

    void Dash(ref Vector3 _movement)
    {
        if (!dashCoolDownActive)
        { //check cooldown to allow dash or not (if true, dash is allowed)
            if (Input.GetButtonDown("Dash"))
            {
                dashCoolDownActive = true;
                dashCurrentSpeed = dashImpulse; //set dash speed to initial dash impulse
            }
        }
        else
        { //update dash cooldown
            dashTimer += Time.deltaTime;

            if (dashTimer < dashDuration)
            { //duration of dash movement
                dashCurrentSpeed = Mathf.Lerp(dashImpulse, 0.0f, dashTimer / dashDuration); //slow down dash speed depending on time
                _movement *= dashCurrentSpeed;
            }

            if (dashTimer >= dashCoolDown)
            { //cooldown to allow dash again
                dashCoolDownActive = false;
                dashTimer = 0.0f;
            }
        }
    }

    void Jump()
    {
        if (onGround && Input.GetKeyDown(InputsManager.Instance.currentInputs.jump))
            verticalSpeed = jumpForce;
    }
    public void AddWeapon(GameObject weapon)
    {
        if (weaponSlot1.empty)
        {
            weapon.transform.parent = weaponSlot1.transform;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weaponSlot1.empty = false;
            activeWeapon = 1;
            weaponSlot1.active = true;
            weaponSlot2.active = false;
            weapon.SendMessage("ActivateCanvas");
            foreach (Transform t in weapon.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Weapon.ToString());
            }
            
        }
        else if (weaponSlot2.empty)
        {
            weapon.transform.parent = weaponSlot2.transform;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weaponSlot2.empty = false;
            activeWeapon = 2;
            weaponSlot1.active = false;
            weaponSlot1.currentWeapon.ChangeWeapon(false);
            weaponSlot2.active = true;
            weapon.SendMessage("ActivateCanvas");
            foreach (Transform t in weapon.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Weapon.ToString());
            }
            
        }

    }
    void ChangeActiveWeapon()
    {
        if (Input.GetKeyDown(InputsManager.Instance.currentInputs.selectWeaponOne))
        {

            activeWeapon = 1;
            weaponSlot2.active = false;
            if (weaponSlot2.currentWeapon != null)
            {

                weaponSlot2.currentWeapon.ChangeWeapon(false);
            }
            weaponSlot1.active = true;
            if (weaponSlot1.currentWeapon != null)
            {

                weaponSlot1.currentWeapon.ChangeWeapon(true);
            }

        }

        if (Input.GetKeyDown(InputsManager.Instance.currentInputs.selectWeaponTwo))
        {

            activeWeapon = 2;
            weaponSlot1.active = false;
            if (weaponSlot1.currentWeapon != null)
            {

                weaponSlot1.currentWeapon.ChangeWeapon(false);
            }
            weaponSlot2.active = true;
            if (weaponSlot2.currentWeapon != null)
            {
                weaponSlot2.currentWeapon.ChangeWeapon(true);
            }
        }
        if(Input.GetAxis("Mouse ScrollWheel") < -0.05f)
        {

            activeWeapon = 1;
            weaponSlot2.active = false;
            if (weaponSlot2.currentWeapon != null)
            {
                weaponSlot2.currentWeapon.ChangeWeapon(false);
            }
            weaponSlot1.active = true;
            if (weaponSlot1.currentWeapon != null)
            {
                weaponSlot1.currentWeapon.ChangeWeapon(true);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0.05f)
        {


            activeWeapon = 2;
            weaponSlot1.active = false;
            if (weaponSlot1.currentWeapon != null)
            {
                weaponSlot1.currentWeapon.ChangeWeapon(false);
            }
            weaponSlot2.active = true;
            if (weaponSlot2.currentWeapon != null)
            {
                weaponSlot2.currentWeapon.ChangeWeapon(true);
            }
        }
    }
    
    void CheckWorldChange()
    {
        if (Input.GetKeyDown(InputsManager.Instance.currentInputs.changeWorld))
        {

            if (onConflictZone == false)
            {
                WorldChangerManager.Instance.PlayerWorldChange(this.gameObject);
                worldChangeTimer = worldChangeTime;
            }
            else
            {
                UIAnimator.Play("AppearText");
            }
        }
    }
    public void OnSave()
    {

        playerData.currentHealth = currentHealth;
        DataManager.gameData.playerData.currentHealth = playerData.currentHealth;
    }

    public void OnLoad()
    {
   
        currentHealth = playerData.currentHealth;
    }
    void OnPause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        pauseOn = true;
    }
    void OffPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseOn = false;
    }

    void Restart() {
        StartCoroutine(cameraShake.Shake(0.2f, 0.4f));
        StartCoroutine(RestartGame(0.21f));
    }

    private void LateUpdate()
    {
        if (respawning) {
            ResetPosition();
            respawning = false;
        }
    }
    void ResetPosition()
    {
        characterController.enabled = false;
        respawnPoint = GameObject.Find("RespawnPoint").transform;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        characterController.enabled = true;
    }

    void TakeDamage()
    {
        if (vulnerable || respawning)
        {
            MainCanvas.Instance.SplashDamage();
            currentHealth--;
            if (!CheckHealth()) {
                vulnerable = false;
                StartCoroutine("InvulnerabilityTimer");
            }
        }
    }

    bool CheckHealth() {
        if (currentHealth <= 0) {
            movementLocked = true;
            Restart();
            return true;
        }
        return false;
    }
    
    public void Heal()
    {
        currentHealth++;
        MainCanvas.Instance.SplashHeal();
        healParticle.Play();
    }

    public IEnumerator RestartGame(float _time) {
        yield return new WaitForSeconds(_time);
        movementLocked = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public IEnumerator InvulnerabilityTimer()
    {
        while (invulnerabilityTimer <= invulnerabilityTime)
        {
            invulnerabilityTimer += Time.deltaTime;
            yield return null;
        }
        invulnerabilityTimer = 0f;
        vulnerable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!respawning && other.gameObject.CompareTag("DeathZone") && other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            if (currentHealth > 1)
                respawning = true;
            TakeDamage();
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        ResetPosition();
    }
}
