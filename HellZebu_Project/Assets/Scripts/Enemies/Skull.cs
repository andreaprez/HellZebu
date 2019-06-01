using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Skull : Enemy
{
    #region VARIABLES
    [SerializeField] private WorldType type;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float evadeSpeed;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeAcceleration;
    [SerializeField] private float angularSpeedForShooting;
    [SerializeField] private float initialCooldownForChasing;
    [SerializeField] private float timeToRecalculateChasing;
    [SerializeField] private float overshootingAcceleration;
    [SerializeField] private bool timePassedStartsOnlyOnPatrol;
    [SerializeField] private float timeToDetect;
    [SerializeField] private float evadeDistance;
    [SerializeField] private float shootingRadius;
    [SerializeField] private float focusRadius;
    [SerializeField] private float focusCooldown;
    [SerializeField] private float chargingDelay;
    [SerializeField] private float shootingRate;
    [SerializeField] private float projectilesSpeed;
    [SerializeField] private EnemyProjectile projectile;
    [SerializeField] private GameObject chargeParticle;

    private enum WorldType { ICE, FIRE };
    private enum State{ PATROL, CHASE, SHOOT, EVADE, FOCUS, CHARGE, DIE};
    [SerializeField] private State currentState;
    private bool initializing = true;
    private float initialTimePassed;
    private float chasingTimePassed;
    private bool countingTimePassed;
    private float timePassed;
    private float focusTimePassed;
    private float shootingTimePassed;
    private float chargingTimePassed;
    private bool playerIsInMyWorld;
    private float timeSinceStateEntered;
    #endregion
    
    void Start()
    {

       

        player = EnemyGlobalBlackboard.player;
        navMeshAgent.speed = normalSpeed;
        navMeshAgent.acceleration = overshootingAcceleration;
        
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
        
        ChangeState(State.PATROL);
    }

    void Update() {
        
        if (!initializing && countingTimePassed) timePassed += Time.deltaTime;
        
        //check emergency situations
        if (healthPoints <= 0f)
            ChangeState(State.DIE);
        if (playerIsInMyWorld && !CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = false;
            countingTimePassed = false;
            timePassed = 0f;
            light.SetActive(false);
        }
        if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) {
            light.SetActive(true);
        }
        //update state
        Vector3 distanceToDestination = transform.position - navMeshAgent.destination;
        distanceToDestination.y = 0f;
        Vector3 distanceToPlayer = transform.position - player.transform.position;
        distanceToPlayer.y = 0f;

        switch (currentState) {
            case  State.PATROL:
                if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) {
                    playerIsInMyWorld = true;
                    if (Vector3.SqrMagnitude(distanceToPlayer) < playerNearbyRadius * playerNearbyRadius) {
                        ChangeState(State.EVADE);
                        if (initializing) initializing = false;
                    }
                    break;
                }
                if (initializing && initialTimePassed >= initialCooldownForChasing) {
                    ChangeState(State.CHASE);
                    initializing = false;
                    break;
                }
                if (!initializing && timePassed >= timeToDetect && CheckPlayerIsInMyWorld()) {
                    ChangeState(State.CHASE);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.PATROL);
                    break;
                }
                if (initializing) initialTimePassed += Time.deltaTime;
                if (playerIsInMyWorld && !countingTimePassed) countingTimePassed = true;
                break;
            case  State.CHASE:
                if (!CheckPlayerIsInMyWorld()) {
                    EnemyGlobalBlackboard.lastPlayerKnownPosition = player.transform.position;
                    ChangeState(State.PATROL);
                    break;
                }
                if (chasingTimePassed >= timeToRecalculateChasing ||
                    Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.CHASE);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToPlayer) < shootingRadius * shootingRadius) {
                    ChangeState(State.SHOOT);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToPlayer) < focusRadius * focusRadius) {
                    ChangeState(State.FOCUS);
                    break;
                }
                chasingTimePassed += Time.deltaTime;
                break;
            case State.SHOOT:
                if (!CheckPlayerIsInMyWorld()) {
                    EnemyGlobalBlackboard.lastPlayerKnownPosition = player.transform.position;
                    ChangeState(State.PATROL);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToPlayer) > shootingRadius * shootingRadius) {
                    ChangeState(State.CHASE);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToPlayer) < focusRadius * focusRadius && focusTimePassed > focusCooldown) {
                    ChangeState(State.FOCUS);
                    break;
                }
                if (shootingTimePassed > 1f/shootingRate) {
                    Shoot();
                    break;
                }
                shootingTimePassed += Time.deltaTime;
                focusTimePassed += Time.deltaTime;
                break;
            case State.FOCUS:
                if (!CheckPlayerIsInMyWorld()) {
                    EnemyGlobalBlackboard.lastPlayerKnownPosition = player.transform.position;
                    ChangeState(State.PATROL);
                    break;
                }
                if (chargingTimePassed > chargingDelay) {
                    ChangeState(State.CHARGE);
                    break;
                }
                chargingTimePassed += Time.deltaTime;
                break;
            case State.CHARGE:
                if (!CheckPlayerIsInMyWorld()) {
                    EnemyGlobalBlackboard.lastPlayerKnownPosition = player.transform.position;
                    ChangeState(State.PATROL);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.CHASE);
                }
                break;
            case  State.EVADE:
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.CHASE);
                    break;
                }
                if (timeSinceStateEntered >= 2f && navMeshAgent.velocity.sqrMagnitude < 0.1f) {
                    ChangeState(State.CHASE);
                    break;
                }
                timeSinceStateEntered += Time.deltaTime;
                break;
            case  State.DIE:
                break;
        }
    }

    // OWN FUNCTIONS
    void Shoot() {
        shootingTimePassed = 0f;
        EnemyProjectile p = Instantiate(projectile, transform.position, transform.rotation);
        p.Speed = projectilesSpeed;
        p.Direction = transform.forward;
        p.CurrentWorld = currentWorld == EWorld.FIRE ? EnemyProjectile.EWorld.FIRE : EnemyProjectile.EWorld.ICE;
    }

    void Charge() {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = chargeSpeed;
        navMeshAgent.acceleration = chargeAcceleration;
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.Normalize();
        navMeshAgent.SetDestination(player.transform.position + directionToPlayer * destinationReachedRadius);
    }
    
    // CHANGE STATE function
    void ChangeState(State newState) {
        switch (currentState) {
            case  State.PATROL:
                navMeshAgent.isStopped = true;
                break;
            case  State.CHASE:
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = normalSpeed;
                chasingTimePassed = 0f;
                break;
            case State.SHOOT:
                navMeshAgent.acceleration = overshootingAcceleration;
                shootingTimePassed = 0f;
                break;
            case State.FOCUS:
                chargingTimePassed = 0f;
                break;
            case State.CHARGE:
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = normalSpeed;
                focusTimePassed = 0f;
                navMeshAgent.acceleration = overshootingAcceleration;
                break;
            case  State.EVADE:
                timeSinceStateEntered = 0f;
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = normalSpeed;
                navMeshAgent.acceleration -= evadeSpeed / normalSpeed;
                break;
            case  State.DIE:
                break;
        }
        switch (newState) {
            case  State.PATROL:
                SetPatrol(EnemyGlobalBlackboard.lastPlayerKnownPosition);
                break;
            case  State.CHASE:
               
                SetChase(player.transform.position);
                navMeshAgent.speed = chasingSpeed;
                timePassed = 0f;
                countingTimePassed = false;
                break;
            case State.SHOOT:
               // FMODUnity.RuntimeManager.PlayOneShot(Attack, transform.position);
                navMeshAgent.acceleration *= 4f;
                break;
            case State.FOCUS:
                chargeParticle.SetActive(true);          
                break;
            case State.CHARGE:
                Charge();
                break;
            case  State.EVADE:
                SetEvade(player.transform.position, evadeDistance);
                navMeshAgent.speed = evadeSpeed;
                navMeshAgent.acceleration += evadeSpeed / normalSpeed;
                if (!timePassedStartsOnlyOnPatrol) countingTimePassed = true;
                break;
            case  State.DIE:
                SetDie(false);
                break;
        }
        
        currentState = newState;
    }

    // UPDATE ROTATION WHEN SHOOTING OR FOCUSING
    private void LateUpdate() {
        if (currentState == State.SHOOT) {
            Vector3 targetDirection = player.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * angularSpeedForShooting);
        }
        else if (currentState == State.FOCUS) {
            Vector3 targetDirection = player.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * focusTimePassed/focusCooldown);
        }
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            DamagePlayer(other.GetComponent<Controller>());
            if (currentState == State.CHARGE) ChangeState(State.DIE);
        }
    }
    
    public void Damage() {
        base.TakeDamage();
        FMODUnity.RuntimeManager.PlayOneShot(BulletCollision, transform.position);

        if (type == WorldType.ICE && currentWorld == EWorld.FIRE ||
            type == WorldType.FIRE && currentWorld == EWorld.ICE) 
            healthPoints--;
    }
}
