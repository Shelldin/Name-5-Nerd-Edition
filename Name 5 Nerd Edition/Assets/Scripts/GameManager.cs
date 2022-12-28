using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    

    //number of players or teams playing
    //[HideInInspector]
    public int playerCountInt = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene("StartMenuScene");
    }

    public void LoadMainGameScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
