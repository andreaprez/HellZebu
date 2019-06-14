using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CentipedeMovement : MonoBehaviour
{
    #region VARIABLES
    private Centipede centipede;
    private float movementSpeed;
    private float horizontalRotSpeed;
    private float timeToChangeDirection;
    private float timeToFullSeek;
    private float changeDirectionTimer;
    private float seekTimer;
    private float currentTime;
    private GameObject player;
    private float seekValue;
    #endregion

    private void Start() {
        centipede = transform.parent.GetComponent<Centipede>();
        movementSpeed = centipede.MovementSpeed;
        timeToChangeDirection = centipede.TimeToChangeDirection;
        timeToFullSeek = centipede.TimeToFullSeek;
        player = centipede.Player;
        
        transform.rotation = Quaternion.Euler(-35f, 0f, 0f);
    }

    void Update()
    {
        // update seek
        seekTimer += Time.deltaTime;
        if (seekTimer < timeToFullSeek)
            seekValue = Mathf.Lerp(0f, 1f, seekTimer / timeToFullSeek);
        else seekValue = 1f;

        // update direction
        currentTime += Time.deltaTime;
        changeDirectionTimer += Time.deltaTime;
        if (changeDirectionTimer >= timeToChangeDirection) {
            changeDirectionTimer = 0f;
            horizontalRotSpeed = Random.Range(-40f, 40f);
        }

        // combine wander/seek
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Vector3 myForward = transform.forward;
        directionToPlayer.y = 0f;
        myForward.y = 0f;
        float angleToPlayer = Vector3.SignedAngle(myForward, directionToPlayer, Vector3.up);
        float rotationHorizontalValue = horizontalRotSpeed * (1f - seekValue) + angleToPlayer * seekValue;
        
        float distanceToPlayerY = player.transform.position.y - transform.position.y;
        float playerOverValue = distanceToPlayerY >= 0f ? -1f : 1f;
        
        //rotate
        if (Time.timeScale != 0f) {
            transform.Rotate(Vector3.right, Mathf.Sin(currentTime) + (0.1f * playerOverValue), Space.Self);
            transform.Rotate(Vector3.up, rotationHorizontalValue * Time.deltaTime, Space.World);
        }
    

        // move
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }
}
