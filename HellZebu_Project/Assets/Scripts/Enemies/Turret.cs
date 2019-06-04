using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy
{
    #region VARIABLES
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float scopeRange;
    [SerializeField] private float chargeTime;
    //[SerializeField] private TurretLaser laserPrefab;
    [SerializeField] private TurretLaser laser;
    [SerializeField] private Transform laserSpawnPoint;
    [SerializeField] private float laserSpeed;
    [SerializeField] private float laserRange;
    [SerializeField] private GameObject movableEye;

    private enum State{ IDLE, ROTATE, SHOOT, DIE };
    [SerializeField] private State currentState;
    private float chargingTimePassed;
    private bool playerIsInMyWorld;
    #endregion

    void Start() {
        player = EnemyGlobalBlackboard.player;

        if (currentWorld == EWorld.FIRE) transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
        else transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
        
        if (CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = true;
            gameObject.layer = LayerMask.NameToLayer(MaskNames.Enemies.ToString());
            light.SetActive(true);
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Enemies.ToString());
            }
        }
        else {
            playerIsInMyWorld = false;
            gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            light.SetActive(false);
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            }
        }
        
        ChangeState(State.IDLE);
    }

    void Update() {
       
        //check emergency situations
        if (healthPoints <= 0f)
            ChangeState(State.DIE);
        if (playerIsInMyWorld && !CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = false;
            chargingTimePassed = 0f;
        }
        else if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = true;
            light.SetActive(true);

        }
        if (playerIsInMyWorld && !CheckPlayerIsInMyWorld()) {
            light.SetActive(false);
        }

        //update state
        switch (currentState) {
            case  State.IDLE:
                if (CheckPlayerIsInMyWorld() && !CheckTargetInScope(player)) {
                    ChangeState(State.ROTATE);
                    break;
                }
                if (CheckPlayerIsInMyWorld() && CheckTargetInScope(player) && chargingTimePassed > chargeTime) {
                    ChangeState(State.SHOOT);
                    break;
                }
                if (playerIsInMyWorld) chargingTimePassed += Time.deltaTime;
                break;
            case  State.ROTATE:
                if (!CheckPlayerIsInMyWorld()) {
                    ChangeState(State.IDLE);
                    break;
                }
                if (CheckTargetInScope(player) && chargingTimePassed > chargeTime) {
                    ChangeState(State.SHOOT);
                    break;
                }
                chargingTimePassed += Time.deltaTime;
                break;
            case State.SHOOT:
                if (!CheckPlayerIsInMyWorld()) {
                    ChangeState(State.IDLE);
                    break;
                }
                if (!laser.gameObject.activeInHierarchy) {
                    ChangeState(State.IDLE);
                }
                /*if (laser == null || laser.Equals(null)) {
                    ChangeState(State.IDLE);
                }*/
                break;
            case State.DIE:
                break;
        }
    }
    
    // OWN FUNCTIONS
    bool CheckTargetInScope(GameObject target) {
        Vector3 directionToTarget = target.transform.position - movableEye.transform.position;
        directionToTarget.Normalize();
        float angleToTarget = Vector3.Angle(movableEye.transform.forward, directionToTarget);
        
        return (angleToTarget < scopeRange / 2f);
    }
    
    void Shoot() {
        laser.gameObject.SetActive(true);
        laser.SpawnPoint = laserSpawnPoint;
        laser.Speed = laserSpeed;
        laser.Range = laserRange;
        laser.Reset();


// !!!!!!! MOVING LASER
        /*laser = Instantiate(laserPrefab, laserSpawnPoint.position, transform.rotation);
        laser.Speed = laserSpeed;
        laser.Direction = transform.forward;
        laser.CurrentWorld = currentWorld == EWorld.FIRE ? EnemyProjectile.EWorld.FIRE : EnemyProjectile.EWorld.ICE;
        laser.Target = player;*/
    }
    
    // CHANGE STATE function
    void ChangeState(State newState) {
        switch (currentState) {
            case State.IDLE:
                break;
            case State.ROTATE:
                break;
            case State.SHOOT:
                break;
            case State.DIE:
                break;
        }
        switch (newState) {
            case State.IDLE:
                break;
            case State.ROTATE:
                break;
            case State.SHOOT:
                chargingTimePassed = 0f;
                Shoot();
                break;
            case State.DIE:
                SetDie(false);
                break;
        }
        
        currentState = newState;
    }
    
    // UPDATE ROTATION WHEN ROTATING OR SHOOTING
    private void LateUpdate() {
        if (currentState == State.ROTATE || currentState == State.SHOOT) {
            Vector3 targetDirection = player.transform.position - movableEye.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            movableEye.transform.rotation = Quaternion.Lerp(movableEye.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
            DamagePlayer(other.GetComponent<Controller>());
    }
    
    public void Damage() {
        base.TakeDamage();
        healthPoints--;
        MainCanvas.Instance.ShowHitmarker();
    }
}
