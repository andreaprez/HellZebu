using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadSceneTrigger : MonoBehaviour
{
    public string SceneName;
    public Animation openDoor;
    public Collider doorCollider;

   private void Awake()
   {
       openDoor.Play();
       doorCollider.enabled = false;
   }

   private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MainCanvas.Instance.sceneToLoad = SceneName;
            MainCanvas.Instance.FadeOut();
        }
    }
}
