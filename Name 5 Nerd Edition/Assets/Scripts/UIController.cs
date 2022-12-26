using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject startMenu,
        playerCountMenu,
        colorSelectMenu;

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

    private void Start()
    {
        startMenu.SetActive(true);
    }

    //Button to start game on the initial screen. Takes you to player count selection
    public void StartGameButton()
    {
        startMenu.SetActive(false);
        playerCountMenu.SetActive(true);
    }

    //select how many players will be playing. Takes you to color selection
    public void SetPlayerCountButton(int playerCount)
    {
        GameManager.instance.playerCountInt = playerCount;
        playerCountMenu.SetActive(false);
    }
}
