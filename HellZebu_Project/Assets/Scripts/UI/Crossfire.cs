using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crossfire : MonoBehaviour
{
    [SerializeField] private List<Image> crossfireImages;
    [SerializeField] private List<Sprite> crossfireRedSprites;
    [SerializeField] private List<Sprite> crossfireBlueSprites;

    public void ChangeCrossfire(bool _fire) {
        if (_fire)
        {
            for (int i = 0; i < crossfireImages.Count; i++)
            {
                crossfireImages[i].sprite = crossfireRedSprites[i];
            }
        }
        else
        {
            for (int i = 0; i < crossfireImages.Count; i++)
            {
                crossfireImages[i].sprite = crossfireBlueSprites[i];
            }
        }
    }


}
