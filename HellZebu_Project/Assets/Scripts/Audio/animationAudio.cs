using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationAudio : MonoBehaviour
{
    // Start is called before the first frame update
    public bool activated = false;
    public FMODUnity.StudioEventEmitter eventEmiiter;
    public Animation myAnimation;
    void Start()
    {
        if (!eventEmiiter)
        {
            eventEmiiter = GetComponent<FMODUnity.StudioEventEmitter>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myAnimation.isPlaying&& !activated)
        {
            eventEmiiter.Play();
            activated = true;
        }
    }
}
