using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject startMenu,
        playerCountMenu,
        colorSelectMenu;

    public TMP_Text colorInstructionText;

    private int playerColorChoiceCoutdown;

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

        //determine how many colors can be selected based on how many teams/players are playing
        playerColorChoiceCoutdown = playerCount;
        
        SetInstructionText();

        playerCountMenu.SetActive(false);
        colorSelectMenu.SetActive(true);
    }
    
    //select color for each player or team's game piece. After all colors are selected begin the game.
    public void SetPlayerColor()
    {

        //choose color and subtract 1 from countdown as long as there are still players left ot choose color
        if (playerColorChoiceCoutdown > 1)
        {
            Debug.Log("Color Choice");
            playerColorChoiceCoutdown--;
            SetInstructionText();
        }
        //move on to main game after final color is chosen
        else if (playerColorChoiceCoutdown == 1)
        {
            Debug.Log(" final color choice");
            colorSelectMenu.SetActive(false);
            //****************** code to load main game scene here  *****************
        }
        
    }
    
    //set the instruction text for which player/team is selecting their color
    private void SetInstructionText()
    {
        colorInstructionText.text = "Team/Player " + playerColorChoiceCoutdown + " choose your color.";
    }

    //make it so a button cannot be selected again after it has already been pressed
    public void DisableButton(Button buttonToDisable)
    {
        buttonToDisable.interactable = false;
    }
    
    
}
