using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
public class InputsManager : MonoBehaviour
{
    public Inputs defaultInputs;

    private Inputs customInputs;
    private string customInputsFile;
    public CurrentInputs currentInputs;
    public static InputsManager Instance;

   
    string customPath;

    private void Awake()
    {
        customPath = Application.persistentDataPath + "/customInputs.json";

        currentInputs = new CurrentInputs();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }


        if (File.Exists(customPath))
        {
            customInputsFile = File.ReadAllText(customPath);
            customInputs = JsonUtility.FromJson<Inputs>(customInputsFile);

        }
        else
        {
            File.CreateText(customPath).Dispose();

            customInputs = defaultInputs;
            customInputsFile = JsonUtility.ToJson(customInputs);
            File.WriteAllText(customPath, customInputsFile);


        }







        StringsToKeyCodes();

    }
    void Update()
    {

    }

    private void StringsToKeyCodes()
    {
        currentInputs.moveForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.moveForward);
        currentInputs.moveBackwards = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.moveBackwards);
        currentInputs.moveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.moveRight);
        currentInputs.moveLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.moveLeft);
        currentInputs.shoot = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.shoot);
        currentInputs.specialShoot = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.specialShoot);
        currentInputs.changeWorld = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.changeWorld);
        currentInputs.selectWeaponOne = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.selectWeaponOne);
        currentInputs.selectWeaponTwo = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.selectWeaponTwo);
        currentInputs.jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.jump);
        currentInputs.transferEnemy = (KeyCode)System.Enum.Parse(typeof(KeyCode), customInputs.transferEnemy);

    }
    public void ResetToDefault()
    {
        customInputs.moveForward = defaultInputs.moveForward;
        customInputs.moveBackwards = defaultInputs.moveBackwards;
        customInputs.moveRight = defaultInputs.moveRight;
        customInputs.moveLeft = defaultInputs.moveLeft;
        customInputs.shoot = defaultInputs.shoot;
        customInputs.specialShoot = defaultInputs.specialShoot;
        customInputs.changeWorld = defaultInputs.changeWorld;
        customInputs.selectWeaponOne = defaultInputs.selectWeaponOne;
        customInputs.selectWeaponTwo = defaultInputs.selectWeaponTwo;
        customInputs.jump = defaultInputs.jump;
        customInputs.transferEnemy = defaultInputs.transferEnemy;

        StringsToKeyCodes();
        SaveCustomInputs();


    }
    public void SaveCustomInputs()
    {
        customInputs.moveForward = currentInputs.moveForward.ToString();
        customInputs.moveBackwards = currentInputs.moveBackwards.ToString();
        customInputs.moveRight = currentInputs.moveRight.ToString();
        customInputs.moveLeft = currentInputs.moveLeft.ToString();
        customInputs.shoot = currentInputs.shoot.ToString();
        customInputs.specialShoot = currentInputs.specialShoot.ToString();
        customInputs.changeWorld = currentInputs.changeWorld.ToString();
        customInputs.selectWeaponOne = currentInputs.selectWeaponOne.ToString();
        customInputs.selectWeaponTwo = currentInputs.selectWeaponTwo.ToString();
        customInputs.jump = currentInputs.jump.ToString();
        customInputs.transferEnemy = currentInputs.transferEnemy.ToString();

        customInputsFile = JsonUtility.ToJson(customInputs);
        File.WriteAllText(customPath, customInputsFile);


    }
}
public class CurrentInputs
{
    public KeyCode moveRight;
    public KeyCode moveLeft;
    public KeyCode moveForward;
    public KeyCode moveBackwards;
    public KeyCode shoot;
    public KeyCode specialShoot;
    public KeyCode changeWorld;
    public KeyCode selectWeaponOne;
    public KeyCode selectWeaponTwo;

    public KeyCode jump;
    public KeyCode transferEnemy;
    public CurrentInputs() { }

}
