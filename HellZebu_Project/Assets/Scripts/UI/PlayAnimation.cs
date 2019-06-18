using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAnimation : MonoBehaviour
{
    public Animation animation;

    void Start()
    {
        Debug.Log("START");
        animation.Rewind();
        animation.Play();
    }
}
