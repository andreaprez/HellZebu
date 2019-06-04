﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGlobalBlackboard : MonoBehaviour
{
    public static GameObject player;
    public static Controller playerController;
    public static Vector3 lastPlayerKnownPosition;
    public static Transform fireHiddenParent;
    public static Transform iceHiddenParent;
    public static List<GameObject> activeOrbsInFire;
    public static List<GameObject> activeOrbsInIce;

    private void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<Controller>();
        player = playerController.gameObject;
        lastPlayerKnownPosition = player.transform.position;
        fireHiddenParent = GameObject.Find("FireHiddenObjects").transform;
        iceHiddenParent = GameObject.Find("IceHiddenObjects").transform;
        activeOrbsInFire = new List<GameObject>();
        activeOrbsInIce = new List<GameObject>();
    }
    
}
