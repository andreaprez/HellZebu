using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatParticleFix : MonoBehaviour
{
    public bool isFire;
    public GameObject particlePos;
    public WeaponSlot slot;

    void Update()
    {
        if (slot.active)
        {
            if (isFire)
            {
                if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Fire)
                {
                    particlePos.SetActive(true);
                }
                else
                {
                    particlePos.SetActive(false);
                }
            }
            else
            {
                if (WorldChangerManager.Instance.currentWorld == WorldChangerManager.Worlds.Fire)
                {
                    particlePos.SetActive(false);
                }
                else
                {
                    particlePos.SetActive(true);
                }

            }

        }
        else
        {
            particlePos.SetActive(false);
        }
       

    }
}
