using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            if (other.GetComponent<Controller>() != null)
            {
                if (other.GetComponent<Controller>().currentHealth < MainCanvas.Instance.UILifes.Length)
                {
                    other.GetComponent<Controller>().Heal();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
