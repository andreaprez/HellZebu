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
    private GameObject levelAttractor;
    private bool returning = false;
    #endregion

    private void Start() {
        centipede = transform.parent.GetComponent<Centipede>();
        movementSpeed = centipede.MovementSpeed;
        timeToChangeDirection = centipede.TimeToChangeDirection;
        timeToFullSeek = centipede.TimeToFullSeek;
        player = centipede.Player;
        levelAttractor = GameObject.Find("LevelAttractor");
        
        transform.rotation = Quaternion.Euler(-35f, 0f, 0f);
    }

    void Update() {
        // update seek
        seekTimer += Time.deltaTime;
        if (seekTimer < timeToFullSeek)
            seekValue = Mathf.Lerp(0f, 1f, seekTimer / timeToFullSeek);
        else seekValue = 1f;
        
        // combine wander/seek
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Vector3 myForward = transform.forward;
        directionToPlayer.y = 0f;
        myForward.y = 0f;
        float angleToPlayer = Vector3.SignedAngle(myForward, directionToPlayer, Vector3.up);
        float rotationHorizontalValue = horizontalRotSpeed * (1f - seekValue) + angleToPlayer * seekValue;
        
        // calculate direction (checking level attractor)
        /*if (seekValue < 1f && (Vector3.Distance(transform.position, levelAttractor.transform.position) > 160f
            || transform.position.y - levelAttractor.transform.position.y > 35f
            || transform.position.y - levelAttractor.transform.position.y < -13f))
            returning = true;
        else if (returning && Vector3.Distance(transform.position, levelAttractor.transform.position) < 2f) {
            returning = false;
            currentTime = 0f;
            transform.rotation = Quaternion.Euler(-35f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }*/
        /*if (returning) {
            Vector3 directionToAttractor = levelAttractor.transform.position - transform.position;
            directionToAttractor.Normalize();
            Vector3 myDirection = transform.forward;
            //float angleToAttractor = Vector3.SignedAngle(myDirection, directionToAttractor, Vector3.one);
            //transform.Rotate(Vector3.up, angleToAttractor * Time.deltaTime, Space.World);
            //transform.rotation = Quaternion.LookRotation(directionToAttractor);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(directionToAttractor), Time.deltaTime * 10f);
                
            
        }
        else {*/
            currentTime += Time.deltaTime;
            changeDirectionTimer += Time.deltaTime;
            if (changeDirectionTimer >= timeToChangeDirection) {
                changeDirectionTimer = 0f;
                if (!returning)
                    horizontalRotSpeed = Random.Range(-40f, 40f);
                else {
                    Vector3 directionToAttractor = levelAttractor.transform.position - transform.position;
                    directionToAttractor.Normalize();
                    float angleToAttractor = Vector3.SignedAngle(transform.forward, directionToAttractor, Vector3.up);
                    horizontalRotSpeed = angleToAttractor;
                }

            }

            float distanceToPlayerY = player.transform.position.y - transform.position.y;
            float playerOverValue = distanceToPlayerY >= 0f ? -1f : 1f;
            if (Time.timeScale != 0f)
                transform.Rotate(Vector3.right, Mathf.Sin(currentTime) + (0.2f * playerOverValue), Space.Self);
            transform.Rotate(Vector3.up, rotationHorizontalValue * Time.deltaTime, Space.World);
        //}

        // move
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }
}
