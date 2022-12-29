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
        colorSelectMenu,
        diceRollMenu;

    public Canvas canvas;

    public TMP_Text colorInstructionText,
        playerTurnText;

    //public List<Image> colorButtonImagesList = new List<Image>();

    public Image colorButtonImage;

    private int playerColorChoiceCoutdown;

    private WaitForSeconds wfs;
    public float playerTurnTextDisplayTime = 5f;

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
        //colorButtonImagesList.AddRange(GameObject.FindGameObjectsWithTag("ColorButton"));
        
        //set the Render Camera to the main camera of the current scene
        canvas = gameObject.GetComponent(typeof(Canvas)) as Canvas;
        canvas.worldCamera = FindObjectOfType(typeof(Camera)) as Camera;


        if (GameManager.instance.currentSceneName == "StartMenuScene")
        {
            startMenu.SetActive(true);
        }

        wfs = new WaitForSeconds(playerTurnTextDisplayTime);
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
            GameManager.instance.gamePieceSOList[playerColorChoiceCoutdown-1].SetPieceColor(colorButtonImage.color);
            playerColorChoiceCoutdown--;
            SetInstructionText();
        }
        //move on to main game after final color is chosen
        else if (playerColorChoiceCoutdown == 1)
        {
            GameManager.instance.gamePieceSOList[playerColorChoiceCoutdown-1].SetPieceColor(colorButtonImage.color);
            colorSelectMenu.SetActive(false);
            GameManager.instance.LoadMainGameScene();
        }
        
    }

    //set the Render Camera to the main camera of the current scene
    public void SetRenderCamera()
    {
        if (canvas == null)
        {
            canvas = gameObject.GetComponent(typeof(Canvas)) as Canvas;
        }
        canvas.worldCamera = FindObjectOfType(typeof(Camera)) as Camera;
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
    
    //Change the active state of the dice menu
    public void ChangeDiceMenuActiveState()
    {
        if (!diceRollMenu.gameObject.activeSelf)
        {
            diceRollMenu.gameObject.SetActive(true);
        }
        else
        {
            diceRollMenu.gameObject.SetActive(false);
        }
    }
    
    //start of turn stuff
    public void StartPlayerTurn()
    {
        StartCoroutine(StartPlayerTurnCoroutine());
    }

    //display text for which team/player's turn it is and then swap to the dice roll menu
    private IEnumerator StartPlayerTurnCoroutine()
    {
        playerTurnText.text = "Team " + GameManager.instance.currentPlayerTurnCount + "\nIt's your turn";
        playerTurnText.gameObject.SetActive(true);

        yield return wfs;
        
        playerTurnText.gameObject.SetActive(false);
        ChangeDiceMenuActiveState();
    }
    
}
