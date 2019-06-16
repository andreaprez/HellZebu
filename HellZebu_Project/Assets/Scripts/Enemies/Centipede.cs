using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Centipede : Enemy
{
    #region VARIABLES
    private enum State{ WANDER, DIE};
    [SerializeField] private State currentState;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float timeToChangeDirection;
    [SerializeField] private float timeToFullSeek;
    [SerializeField] private CentipedeMovement movementBehaviour;
    [SerializeField] private List<Rigidbody> bodyBlocks;
    [SerializeField] private List<Collider> bodyColliders;
    [SerializeField] private List<HingeJoint> joints;
    [SerializeField] private GameObject changeWorldParticleFire;
    [SerializeField] private GameObject changeWorldParticleIce;
    [SerializeField] private AutoDestroy autoDestroy;

    [HideInInspector] public List<GameObject> weakPoints;
    private bool playerIsInMyWorld;
    private float worldChangeTimer;
    private int currentBodyBlock;
    private bool changing;
    #endregion

    //CentipedeMovement uses these getters
    #region GETTERS 
    public float MovementSpeed { get { return movementSpeed; } }
    public float TimeToChangeDirection { get { return timeToChangeDirection; } }
    public float TimeToFullSeek { get { return timeToFullSeek; } }
    public GameObject Player { get { return player; } }

    #endregion 
    
    private void Awake() {
        weakPoints = new List<GameObject>();
        player = EnemyGlobalBlackboard.player;
        
        if (currentWorld == EWorld.FIRE) transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
        else transform.parent = EnemyGlobalBlackboard.iceHiddenParent;

        if (CheckPlayerIsInMyWorld()) {
            playerIsInMyWorld = true;
            gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            }
        }
        else {
            playerIsInMyWorld = false;
            gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            }
        }
        
        ChangeState(State.WANDER);
    }

    void Update()
    {
        switch (currentState) {
            case  State.WANDER:
                if (playerIsInMyWorld && !CheckPlayerIsInMyWorld()) playerIsInMyWorld = false;
                if (!playerIsInMyWorld && CheckPlayerIsInMyWorld()) playerIsInMyWorld = true;
                if (!playerIsInMyWorld && !changing) worldChangeTimer += Time.deltaTime;
                if (worldChangeTimer >= 1f && !changing) {
                    worldChangeTimer = 0f;
                    changing = true;
                    StartCoroutine("ChangeWorld");
                    if (currentWorld == EWorld.FIRE) {
                        changeWorldParticleFire.SetActive(true);
                        changeWorldParticleFire.transform.position = bodyBlocks[0].transform.position + bodyBlocks[0].transform.forward * 5f;
                        changeWorldParticleFire.transform.localRotation = bodyBlocks[0].transform.localRotation;
                    }
                    else
                    {
                        changeWorldParticleIce.SetActive(true);
                        changeWorldParticleIce.transform.position = bodyBlocks[0].transform.position + bodyBlocks[0].transform.forward * 5f;
                        changeWorldParticleIce.transform.localRotation = bodyBlocks[0].transform.localRotation;
                    }
                }
                break;
            case State.DIE:
                break;
        }
    }
    
    // CHANGE STATE function
    void ChangeState(State newState) {
        switch (currentState) {
            case  State.WANDER:
                movementBehaviour.enabled = false;
                break;
            case State.DIE:
                break;
        }
        switch (newState) {
            case  State.WANDER:
                movementBehaviour.enabled = true;
                break;
            case State.DIE:
                movementBehaviour.enabled = false;
                foreach (Collider collider in bodyColliders) {
                    collider.isTrigger = false;
                }
                foreach (HingeJoint joint in joints) {
                    joint.breakForce = 0f;
                }
                foreach (Rigidbody block in bodyBlocks) {
                    block.useGravity = true;
                    block.isKinematic = false;
                    block.mass = 1.5f;
                    block.drag = 0.5f;
                    block.velocity = Random.Range(-4f, 4f) * Vector3.one;
                }
                StopCoroutine("ChangeWorld");
                if (currentWorld == EWorld.FIRE) changeWorldParticleFire.SetActive(false);
                else changeWorldParticleIce.SetActive(false);

                autoDestroy.enabled = true;
                this.enabled = false;
                break;
        }
        
        currentState = newState;
    }

    public void PlayerCollision(Controller player) {
        DamagePlayer(player,"centipedeCollisions");
    }

    public void Damage() {
        base.TakeDamage();
        if (weakPoints.Count <= 0f)
            ChangeState(State.DIE);
    }
    
    public IEnumerator ChangeWorld() {

        // update hiddenParent
        if (currentWorld == EWorld.ICE)
            bodyBlocks[currentBodyBlock].gameObject.transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
        else bodyBlocks[currentBodyBlock].gameObject.transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
      
        // update layer to hidden
        if (bodyBlocks[currentBodyBlock].gameObject.layer == LayerMask.NameToLayer(MaskNames.Default.ToString())) {
            bodyBlocks[currentBodyBlock].gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            foreach (Transform t in bodyBlocks[currentBodyBlock].gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            }
        }
        else {
            bodyBlocks[currentBodyBlock].gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            foreach (Transform t in bodyBlocks[currentBodyBlock].gameObject.GetComponentsInChildren<Transform>()) {
                t.gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            }
        }

        currentBodyBlock++; // next body block
        
        if (currentBodyBlock < bodyBlocks.Count) {
            // wait and recall function to change next body block
            yield return new WaitForSeconds(0.4f);
            StartCoroutine("ChangeWorld");
        }
        else {
            // change parent and finish
            currentWorld = currentWorld == EWorld.FIRE ? EWorld.ICE : EWorld.FIRE; 
            if (currentWorld == EWorld.FIRE) transform.parent = EnemyGlobalBlackboard.fireHiddenParent;
            else transform.parent = EnemyGlobalBlackboard.iceHiddenParent;
            if (gameObject.layer == LayerMask.NameToLayer(MaskNames.Default.ToString())) {
                gameObject.layer = LayerMask.NameToLayer(MaskNames.HideFromCamera.ToString());
            }
            else {
                gameObject.layer = LayerMask.NameToLayer(MaskNames.Default.ToString());
            }
            
            currentBodyBlock = 0;
            changing = false;
            foreach (Rigidbody bodyBlock in bodyBlocks) {
                bodyBlock.gameObject.transform.parent = transform;
            }
 
        }
    }
}
