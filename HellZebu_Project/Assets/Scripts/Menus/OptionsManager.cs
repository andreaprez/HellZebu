using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
public class OptionsManager : MonoBehaviour
{
    public OptionsConfiguration defaultOptions;
    public OptionsConfiguration customOptions;
    private string customOptionsFile;

   
    string customPath;
    public static OptionsManager Instance;

    public Slider bright;
    public Toggle superVFX;
    public Toggle fullScreen;
    public Slider FOV;
    public Toggle invertXAxis;
    public Toggle leftHanded;
    public Slider mouseSensitivity;


    private void Awake()
    {
      
        customPath = Application.persistentDataPath + "/customOptionsConfig.json";
        Instance = this;
        

        if (File.Exists(customPath))
        {
            customOptionsFile = File.ReadAllText(customPath);
           
            customOptions= JsonUtility.FromJson<OptionsConfiguration>(customOptionsFile);

        }
        else
        {
            print("CREATE");
            //Usar Dispose() despues de crear un archivo si vamos a leer o escribir en el.
            File.CreateText(customPath).Dispose();
            
            customOptions = defaultOptions;
            customOptionsFile = JsonUtility.ToJson(customOptions);
            File.WriteAllText(customPath, customOptionsFile);



        }
       

        SetOptionsValues();


    }
   
    public void SetOptionsValues()
    {

        bright.value = customOptions.bright;
        superVFX.isOn = customOptions.superVFX;
        fullScreen.isOn = customOptions.fullScreen;
        FOV.value = customOptions.FOV;
        invertXAxis.isOn = customOptions.invertXAxis;
        leftHanded.isOn = customOptions.leftHanded;
        mouseSensitivity.value = customOptions.mouseSensitivity;
        print(customOptions.mouseSensitivity);
    }
    public void SaveOptionsValues()
    {
        customOptions.bright = bright.value;
        customOptions.superVFX = superVFX.isOn;
        customOptions.fullScreen = fullScreen.isOn;
        customOptions.FOV = FOV.value;
        customOptions.invertXAxis = invertXAxis.isOn;
        customOptions.leftHanded = leftHanded.isOn;
        customOptions.mouseSensitivity = mouseSensitivity.value;
        customOptionsFile = JsonUtility.ToJson(customOptions);
        File.WriteAllText(customPath, customOptionsFile);

    }
    public void SetToDefault()
    {

        customOptions.bright = defaultOptions.bright;
        customOptions.superVFX = defaultOptions.superVFX;
        customOptions.fullScreen = defaultOptions.fullScreen;
        customOptions.FOV = defaultOptions.FOV;
        customOptions.invertXAxis = defaultOptions.invertXAxis;
        customOptions.leftHanded = defaultOptions.leftHanded;
        customOptions.mouseSensitivity = defaultOptions.mouseSensitivity;

     
        SetOptionsValues();
    }
   

}
