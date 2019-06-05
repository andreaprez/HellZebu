using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   #region VARIABLES
   public EWorld InitialWorld { set { currentWorld = value; } }
   [SerializeField] protected float healthPoints;
   [SerializeField] protected NavMeshAgent navMeshAgent;
   [SerializeField] protected float flockingDistanceFromAttractor;
   [SerializeField] protected float playerNearbyRadius;
   [SerializeField] protected float destinationReachedRadius;
   [SerializeField] protected GameObject light;

   public enum EWorld { ICE, FIRE };
   public EWorld currentWorld;
   protected GameObject player;
    [Header("Sounds")]
    [FMODUnity.EventRef]
    public string BulletCollision = "";
    [FMODUnity.EventRef]
    public string Idle = "";
    public FMOD.Studio.EventInstance IdleInstance;
    [FMODUnity.EventRef]
    public string Attack = "";
    #endregion
    public delegate void EnemeyKill();
    public static event EnemeyKill enemyKillEvent;

   
    protected void SetPatrol(Vector3 attractorPoint) {
      navMeshAgent.isStopped = false;
      Vector3 randomVector = new Vector3(Random.Range(0f, 1f), 0f, Random.Range(0f, 1f));
      Vector3 randomPoint = attractorPoint + randomVector * flockingDistanceFromAttractor;
      navMeshAgent.SetDestination(randomPoint);
   }
   protected void SetChase(Vector3 target) {
      navMeshAgent.isStopped = false;
      navMeshAgent.SetDestination(target);
   }
   protected void SetEvade(Vector3 targetToEvade, float evadeDistance) {
      navMeshAgent.isStopped = false;
      Vector3 directionToTarget = targetToEvade - transform.position;
      directionToTarget.Normalize();
      directionToTarget.y = 0f;
      Vector3 targetPosition = transform.position - directionToTarget * evadeDistance;
      navMeshAgent.SetDestination(targetPosition);
   }

   protected void SetDie(bool _orb) {
      if (_orb) {
         if (currentWorld == EWorld.ICE) EnemyGlobalBlackboard.activeOrbsInIce.Remove(gameObject);
         else EnemyGlobalBlackboard.activeOrbsInFire.Remove(gameObject);
      }
        IdleInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
       

        Destroy(gameObject);
   }

   public void ChangeWorld() {
      currentWorld = currentWorld == EWorld.FIRE ? EWorld.ICE : EWorld.FIRE;
      // change area mask - not working, cannot bake two independent areas / nav meshes
      //navMeshAgent.areaMask = currentWorld == EWorld.FIRE ? 8 : 16; // 8 and 16 are values for ice and fire masks in AreaMask

      // update orbs list for regrouping
      // update hiddenParent
      if (currentWorld == EWorld.FIRE) {
         if (GetComponent<Orb>() != null) {
            EnemyGlobalBlackboard.activeOrbsInIce.Remove(gameObject);
            EnemyGlobalBlackboard.activeOrbsInFire.Add(gameObject);
         }
         transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
      }
      else {
         if (GetComponent<Orb>() != null) {
            EnemyGlobalBlackboard.activeOrbsInFire.Remove(gameObject);
            EnemyGlobalBlackboard.activeOrbsInIce.Add(gameObject);
         }
         transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
      }
      
      // update layer to hidden
      if (gameObject.layer == LayerMask.NameToLayer(MaskNames.Enemies.ToString())) {
         gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
         light.SetActive(false);
         foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
            t.gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
         }
      }
      else {
         gameObject.layer = LayerMask.NameToLayer(MaskNames.Enemies.ToString());
         light.SetActive(true);
         foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
            t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Enemies.ToString());
         }
      }
   }

   protected bool CheckPlayerIsInMyWorld() {
      if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Ice && currentWorld == EWorld.ICE)
         return true;
      else if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Fire && currentWorld == EWorld.FIRE)
         return true;
      else return false;
   }

   protected void DamagePlayer(Controller player) {
         player.SendMessage("TakeDamage");
   }
   
   protected void TakeDamage() { }
   private void OnDestroy()
   {
      enemyKillEvent();   
   }
}
