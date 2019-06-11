using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChangeWorld : MonoBehaviour
{
    // Start is called before the first frame update
    //[FMODUnity.EventRef]
   // public string myEvent = "";
   // private FMOD.Studio.EventInstance musicEvent;
    public bool activated;
   public FMODUnity.StudioEventEmitter eventEmiiter;
    void Start()
    {
      //  musicEvent = FMODUnity.RuntimeManager.CreateInstance(myEvent);
      //  musicEvent.start();
      //  musicEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        activated = true;

        if (!eventEmiiter)
        {
            eventEmiiter = GetComponent<FMODUnity.StudioEventEmitter>();
       //     Debug.Log("getting compont");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.layer == 9 && activated)
        {
            Debug.Log("stopping");
            // musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //musicEvent.release();
            eventEmiiter.Stop();
          

            activated = false;
        }
        else if (this.gameObject.layer != 9 && !activated)
        {
            Debug.Log("playing");
            // musicEvent.start();
            eventEmiiter.Play();

            activated = true;
        }
    }
}
