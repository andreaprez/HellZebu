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
          
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        }
    }
}
