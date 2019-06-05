using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadSceneTrigger : MonoBehaviour
{
   public string SceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            MainCanvas.Instance.sceneToLoad = SceneName;
            MainCanvas.Instance.FadeOut();
        }
    }
}
