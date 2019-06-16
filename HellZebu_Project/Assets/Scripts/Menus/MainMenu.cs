using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject configPanel;
    public GameObject bindingPanel;
    public GameObject startButtons;
    public List<Button> buttons;
    Event keyEvent;
    KeyCode newKey;
    bool waitingForKey;
    bool waitOut;
    [FMODUnity.EventRef]
     public string confirmOption = "";
    [FMODUnity.EventRef]
    public string selectOption = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void PlayGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

#else
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        Application.Quit();
#endif

    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(1);

        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene(2);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene(3);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(5);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SceneManager.LoadScene(6);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SceneManager.LoadScene(7);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SceneManager.LoadScene(8);
        }
    }
    public void SetActiveStartButtons(bool active)
    {
        if (active)
        {
            startButtons.SetActive(true);
        }
        else
        {
            startButtons.SetActive(false);
        }
    }
    public void SetActiveConfigPanel(bool active)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);
        if (active)
        {
            configPanel.SetActive(true);

        }
        else
        {
            configPanel.SetActive(false);
            OptionsManager.Instance.SetOptionsValues();
        }
    }
    public void SetActiveBindingPanel(bool active)
    {
        FMODUnity.RuntimeManager.PlayOneShot(confirmOption);

        if (active)
        {
            bindingPanel.SetActive(true);

        }
        else
        {
            bindingPanel.SetActive(false);
        }
    }

    public void UpdateKeyText()
    {
        foreach (Button b in buttons)
        {
            //  print(b.name);
            if (b.name.Contains("Forward"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveForward.ToString();
            }
            if (b.name.Contains("Left"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveLeft.ToString();
            }
            if (b.name.Contains("Back"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveBackwards.ToString();
            }
            if (b.name.Contains("Right"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.moveRight.ToString();
            }
            if (b.name.Contains("Jump"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.jump.ToString();
            }
            if (b.name.Contains("Shoot"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.shoot.ToString();
            }
            if (b.name.Contains("Special"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.specialShoot.ToString();
            }
            if (b.name.Contains("Transfer"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.transferEnemy.ToString();
            }
            if (b.name.Contains("Weapon1"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.selectWeaponOne.ToString();
            }
            if (b.name.Contains("Weapon2"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.selectWeaponTwo.ToString();
            }
            if (b.name.Contains("WorldChange"))
            {
                b.GetComponentInChildren<Text>().text = InputsManager.Instance.currentInputs.changeWorld.ToString();
            }

        }
    }
    public void ResetInputs()
    {
        InputsManager.Instance.ResetToDefault();
        UpdateKeyText();
    }
    private void OnGUI()
    {

        keyEvent = Event.current;

        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
            print("NEW KEY: " + newKey);
            waitOut = true;
        }
        if (keyEvent.type == EventType.MouseDown && waitingForKey)
        {
            newKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + keyEvent.button);
            waitingForKey = false;
            print("NEW KEY: " + newKey);
            waitOut = true;
        }



    }
   
    public void EnterKey(string keyName)
    {
    
        if (waitingForKey == false)
        {

            StartCoroutine(CheckKey(keyName));

        }
    }
    private IEnumerator Wait()
    {

        while (waitOut == false)
        {
            yield return null;
        }


    }
    public IEnumerator CheckKey(string name)
    {
        waitingForKey = true;
        waitOut = false;
        yield return Wait();


        switch (name)
        {
            case "Forward":
                InputsManager.Instance.currentInputs.moveForward = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Left":
                InputsManager.Instance.currentInputs.moveLeft = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Back":
                InputsManager.Instance.currentInputs.moveBackwards = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Right":
                InputsManager.Instance.currentInputs.moveRight = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Jump":
                InputsManager.Instance.currentInputs.jump = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Shoot":
                InputsManager.Instance.currentInputs.shoot = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Transfer":
                InputsManager.Instance.currentInputs.transferEnemy = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Weapon1":
                InputsManager.Instance.currentInputs.selectWeaponOne = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Weapon2":
                InputsManager.Instance.currentInputs.selectWeaponTwo = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "WChange":
                InputsManager.Instance.currentInputs.changeWorld = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;
            case "Special":
                InputsManager.Instance.currentInputs.specialShoot = newKey;
                InputsManager.Instance.SaveCustomInputs();
                UpdateKeyText();
                break;

        }

    }
}
