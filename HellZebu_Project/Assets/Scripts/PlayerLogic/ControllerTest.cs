using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerTest : MonoBehaviour
{
    #region VARIABLES
    [Header("Camera Control")] 
    [SerializeField] private bool invertedPitch = false;
    [SerializeField] private bool invertedYaw = false;
    [SerializeField] private float pitchRotationalSpeed;
    [SerializeField] private float yawRotationalSpeed;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private Transform pitchController;
    private float pitch;
    private float yaw;
    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTiltAngle;
    [SerializeField] private float movementTiltSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private bool dashEnabled;
    [SerializeField] private float dashImpulse;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private float dashDuration;
    private float dashTimer = 0.0f;
    private bool dashCoolDownActive = false;
    private float dashCurrentSpeed;
    private float verticalSpeed;
    private bool onGround;
    [Header("World Change")]
    [HideInInspector] public bool onConflictZone;
    public float worldChangeTime;
    public float worldChangeTimer;
    [Header("Weapons")]
    public WeaponSlot weaponSlot1;
    public WeaponSlot weaponSlot2;
    public int activeWeapon;
    [Header("Spawn")] 
    //Da error al cargar escena porque ese transform se desvincula del controller al destruirse, lo hardcodeo de momento y ya veremos como lo hacemos
    [SerializeField] private Vector3 respawnPoint= new Vector3(-14f,50f,-134f);
    private bool respawning;
    #endregion

    #region TEMP DATA
    [Header("Health (temp)")] 
    public int currentHealth = 5;
    #endregion

    
    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }
   

    void Update() {
        
// !!!!!!!!!!!!! TEMP HEALTH MANAGEMENT
        if (currentHealth <= 0) {
            Restart();
        }
        if (currentHealth == 1)
        {
            Restart();
        }
        ChangeActiveWeapon();
        Rotate();
        Move();

        if (worldChangeTimer > 0) { worldChangeTimer -= Time.deltaTime; }
        if (worldChangeTimer <=0) {
            CheckWorldChange();
        }
    }

    private void LateUpdate() {
        if (respawning) {
            respawning = false;
            transform.position = respawnPoint;
            transform.rotation =  Quaternion.identity;
        }
    }

    //Rotate camera (pitch + yaw) using mouse input
    void Rotate() {
        float mouseAxisY = Input.GetAxis("Mouse Y");
        float mouseAxisX = Input.GetAxis("Mouse X");

        if (invertedPitch) mouseAxisY = -mouseAxisY;
        pitch += mouseAxisY * pitchRotationalSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        
        if (invertedYaw) mouseAxisX = -mouseAxisX;
        yaw += mouseAxisX * yawRotationalSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
    }
    
    //Move player
    void Move() {
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
        if ((collisionFlags & CollisionFlags.Below) != 0) {
            onGround = true;
            verticalSpeed = 0.0f;
        } 
        else onGround = false;
        if ((collisionFlags & CollisionFlags.Above) != 0 && verticalSpeed > 0.0f)
            verticalSpeed = 0.0f;

        verticalSpeed += gravity * Time.deltaTime;
    }

    void Dash(ref Vector3 _movement)
    {
        if (!dashCoolDownActive) { //check cooldown to allow dash or not (if true, dash is allowed)
            if (Input.GetButtonDown("Dash")) {
                dashCoolDownActive = true;
                dashCurrentSpeed = dashImpulse; //set dash speed to initial dash impulse
            }
        }
        else { //update dash cooldown
            dashTimer += Time.deltaTime;

            if (dashTimer < dashDuration) { //duration of dash movement
                dashCurrentSpeed = Mathf.Lerp(dashImpulse, 0.0f, dashTimer / dashDuration); //slow down dash speed depending on time
                _movement *= dashCurrentSpeed;
            }

            if (dashTimer >= dashCoolDown) { //cooldown to allow dash again
                dashCoolDownActive = false;
                dashTimer = 0.0f;
            }
        }
    }

    void Jump() {
        if (onGround && Input.GetButtonDown("Jump"))
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
        }

    }
    void ChangeActiveWeapon()
    {
        if (Input.GetButtonDown("Weapon Slot 1"))
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

        if (Input.GetButtonDown("Weapon Slot 2"))
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
        if (Input.GetButtonDown("World Change"))
        {

            if (onConflictZone == false)
            {
                WorldChangerManagerTest.Instance.PlayerWorldChange(this.gameObject);
                worldChangeTimer = worldChangeTime;
            }
        }
    }
    
    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        currentHealth = 5;
    }

    void ResetPosition() {
        respawning = true;
    }

    void TakeDamage() {
        currentHealth--;
    }

    private void OnTriggerEnter(Collider other) {
        if (!respawning && other.gameObject.CompareTag("DeathZone")) {
            TakeDamage();
            ResetPosition();
        }
    }
}