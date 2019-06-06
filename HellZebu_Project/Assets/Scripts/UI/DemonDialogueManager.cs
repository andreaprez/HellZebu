using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemonDialogueManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> dialogues;
    [SerializeField] private float dialoguesDuration;

    private Controller playerController;
    private int currentScene;
    private float timer;
    void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<Controller>();
        playerController.movementLocked = true;
        currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene > 0)
        {
            dialogues[currentScene - 1].SetActive(true);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dialoguesDuration)
        {
            dialogues[currentScene - 1].SetActive(false);
            MainCanvas.Instance.FadeIn();
            playerController.movementLocked = false;
            enabled = false;
        }
    }
}
