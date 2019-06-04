using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Orb : Enemy
{
    #region VARIABLES
    [SerializeField] private float normalSpeed;
    [SerializeField] private float chasingSpeed;
    [SerializeField] private float evadeSpeed;
    [SerializeField] private float timeToRecalculateChasing;
    [SerializeField] private float overshootingAcceleration;
    [SerializeField] private bool timePassedStartsOnlyOnPatrol;
    [SerializeField] private float timeToDetect;
    [SerializeField] private float timeToRecover;
    [SerializeField] private float evadeDistance;
    [SerializeField] private GameObject deathParticles;

    private enum State{ PATROL, CHASE, REGROUP, EVADE, DIE};
    [SerializeField] private State currentState;
    private float chasingTimePassed;
    private float regroupingTimePassed;
    private bool countingTimePassed;
    private float timePassed;
    private Vector3 centerOfGroup = Vector3.zero;
    private float groupCounter;
    private bool playerIsInMyWorld;
    private float timeSinceStateEntered;
    #endregion
    
    void Start() {

//IdleInstance = FMODUnity.RuntimeManager.CreateInstance(Idle);
  //      FMODUnity.RuntimeManager.AttachInstanceToGameObject(IdleInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
    //    IdleInstance.start();

        player = EnemyGlobalBlackboard.player;
        navMeshAgent.speed = normalSpeed;
        navMeshAgent.acceleration = overshootingAcceleration;
        
        if (currentWorld == EWorld.FIRE) {
            EnemyGlobalBlackboard.activeOrbsInFire.Add(gameObject);
            transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
        }
        else {
            EnemyGlobalBlackboard.activeOrbsInIce.Add(gameObject);
            transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
        }

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
        
        if (countingTimePassed) timePassed += Time.deltaTime;
        
        //check emergency situations
        if (healthPoints <= 0f)
            ChangeState(State.DIE);
        if (playerIsInMyWorld && !CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = false;
            countingTimePassed = false;
            timePassed = 0f;
            light.SetActive(false);
        }
        if (!playerIsInMyWorld && CheckPlayerIsInMyWorld())
            light.SetActive(true);
        
        //update state
        Vector3 distanceToDestination = transform.position - navMeshAgent.destination;
        distanceToDestination.y = 0f;
        Vector3 distanceToPlayer = transform.position - player.transform.position;
        distanceToPlayer.y = 0f;
        
        switch (currentState) {
            case  State.PATROL:
                if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) {
                    playerIsInMyWorld = true;
                    if (Vector3.SqrMagnitude(distanceToPlayer) < playerNearbyRadius * playerNearbyRadius)
                        ChangeState(State.EVADE);
                    break;
                }
                if (timePassed >= timeToDetect && CheckPlayerIsInMyWorld()) {
                    ChangeState(State.CHASE);
                    break;
                }
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.PATROL);
                    break;
                }
                if (playerIsInMyWorld && !countingTimePassed) countingTimePassed = true;
                break;
            case  State.CHASE:
                if (!CheckPlayerIsInMyWorld()) {
                    EnemyGlobalBlackboard.lastPlayerKnownPosition = player.transform.position;
                    ChangeState(State.REGROUP);
                    break;
                }
                if (chasingTimePassed >= timeToRecalculateChasing ||
                    Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.CHASE);
                    break;
                }
                chasingTimePassed += Time.deltaTime;
                break;
            case  State.REGROUP:
                if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) {
                    playerIsInMyWorld = true;
                    if (Vector3.SqrMagnitude(distanceToPlayer) < playerNearbyRadius * playerNearbyRadius)
                        ChangeState(State.EVADE);
                    break;
                }
                if (regroupingTimePassed >= timeToRecover) {
                    ChangeState(State.PATROL);
                    break;
                }
                regroupingTimePassed += Time.deltaTime;
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    Regroup();
                }
                break;
            case  State.EVADE:
                if (Vector3.SqrMagnitude(distanceToDestination) < destinationReachedRadius * destinationReachedRadius) {
                    ChangeState(State.REGROUP);
                    break;
                }
                if (timeSinceStateEntered >= 2f && navMeshAgent.velocity.sqrMagnitude < 0.1f) {
                    ChangeState(State.REGROUP);
                    break;
                }
                timeSinceStateEntered += Time.deltaTime;
                break;
            case  State.DIE:
                break;
        }
    }

    // OWN FUNCTIONS
    void Regroup() {
        navMeshAgent.isStopped = false;
        centerOfGroup = Vector3.zero;
        groupCounter = 0f;
        if (currentWorld == EWorld.ICE) {
            foreach (GameObject orb in EnemyGlobalBlackboard.activeOrbsInIce) {
                centerOfGroup += orb.transform.position;
                groupCounter++;
            }
        }
        else {
            foreach (GameObject orb in EnemyGlobalBlackboard.activeOrbsInFire) {
                centerOfGroup += orb.transform.position;
                groupCounter++;
            }
        }

        if (groupCounter < 2) ChangeState(State.PATROL);
        else {
            centerOfGroup /= groupCounter;
            navMeshAgent.SetDestination(centerOfGroup);
        }
    }


    // CHANGE STATE function
    void ChangeState(State newState) {
        switch (currentState) {
            case  State.PATROL:

                break;
            case  State.CHASE:
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = normalSpeed;
                chasingTimePassed = 0f;
                break;
            case  State.REGROUP:
                regroupingTimePassed = 0f;
                navMeshAgent.isStopped = true;
                break;
            case  State.EVADE:
                timeSinceStateEntered = 0f;
                navMeshAgent.isStopped = true;
                navMeshAgent.speed = normalSpeed;
                navMeshAgent.acceleration = overshootingAcceleration;
                break;
            case  State.DIE:
                break;
        }
        switch (newState) {
            case  State.PATROL:
                SetPatrol(EnemyGlobalBlackboard.lastPlayerKnownPosition);
                break;
            case  State.CHASE:
                //sound provisionall        
               
                SetChase(player.transform.position);
                navMeshAgent.speed = chasingSpeed;
                timePassed = 0f;
                countingTimePassed = false;
                /*if (Random.Range(0, 10) < 2)
                {
                    Debug.Log(IdleInstance.isValid());
                    IdleInstance.setVolume(50f);

                    IdleInstance.start();

                }*/
                break;
            case  State.REGROUP:
                Regroup();
                break;
            case  State.EVADE:
                SetEvade(player.transform.position, evadeDistance);
                navMeshAgent.speed = evadeSpeed;
                navMeshAgent.acceleration = overshootingAcceleration;
                if (!timePassedStartsOnlyOnPatrol) countingTimePassed = true;
                break;
            case  State.DIE:
               // FMODUnity.RuntimeManager.PlayOneShot(BulletCollision, transform.position);
                Instantiate(deathParticles, transform.position, Quaternion.identity);
                SetDie(true);
                break;
        }
        
        currentState = newState;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && EnemyGlobalBlackboard.playerController.Vulnerable) {
            DamagePlayer(other.GetComponent<Controller>());
            if (currentState == State.CHASE) ChangeState(State.DIE);
        }
    }

    public void Damage() {
        base.TakeDamage();
        healthPoints--;
    }
   
}
