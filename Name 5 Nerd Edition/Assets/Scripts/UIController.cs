using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject startMenu,
        playerCountMenu,
        colorSelectMenu,
        diceRollMenu,
        standardSpaceMenu,
        allPlaySpaceMenu,
        flipFlopSpaceMenu,
        wildSpaceMenu,
        doubleDownSpaceMenu,
        finalSpaceMenu,
        timerTextObj;

    public float standardCountdownTime = 30f,
        flipLFlopCountdownTime = 10f,
        wildSpaceCountdownTime = 30f,
        categorySelectTime = 15f;

    public Button diceButton;

    public Canvas canvas;

    public TMP_Text colorInstructionText,
        playerTurnText,
        flipFlopTeamText,
        wildSpaceText,
        timerText;

    //public List<Image> colorButtonImagesList = new List<Image>();

    public Image colorButtonImage;

    private int playerColorChoiceCoutdown;

    private WaitForSeconds wfs;
    public float timeDelay = 3f;

    public Coroutine standardCountdownTimerCo,
        flipFlopCountdownTimerCo,
        wildSpaceCountdownTimerCo,
        winTimeCountdownTimerCo,
        wildSpaceCategorySelectionTimerCo;

    public CategoryManager categoryManager;
    public List<SelectWildSpaceCategoryEvent> selectWildSpaceEventList = new List<SelectWildSpaceCategoryEvent>();
    public List<Image> categoryBackgroundImageList = new List<Image>();
    public List<TMP_Text> categoryTextUIList = new List<TMP_Text>();


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

        wfs = new WaitForSeconds(timeDelay);
        
        standardCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(standardCountdownTime));
        flipFlopCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(flipLFlopCountdownTime));
        wildSpaceCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(wildSpaceCountdownTime));
        winTimeCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(GameManager.instance.timeToWin));
        wildSpaceCategorySelectionTimerCo = StartCoroutine(NameFiveCountdownCoroutine(categorySelectTime));

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
    
    //Set Dice Menu to Active
    public void SetDiceMenuActive()
    {
        if (!diceRollMenu.gameObject.activeSelf)
        {
            diceRollMenu.gameObject.SetActive(true);
        }
    }
    
    //Set Dice Menu to Inactive
    public void SetDiceMenuInactive()
    {
        if(diceRollMenu.gameObject.activeSelf)
        {
            diceRollMenu.gameObject.SetActive(false);
        }
    }
    
    
    
    //start of turn stuff
    public void StartPlayerTurn()
    {
        SwapRenderModeToOverlay();
        StartCoroutine(StartPlayerTurnCoroutine());
    }

    //display text for which team/player's turn it is and then swap to the dice roll menu
    private IEnumerator StartPlayerTurnCoroutine()
    {
        playerTurnText.text = "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + "\nIt's your turn";
        playerTurnText.gameObject.SetActive(true);
        diceButton.interactable = true;

        yield return wfs;
        
        playerTurnText.gameObject.SetActive(false);
        //if the player is not on the final space take turn as normal
        if (!GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].onFinalSpace)
        {
            SetDiceMenuActive();
        }
        //if player is on final space do final space magic
        else if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].onFinalSpace)
        {
            //temporary final space magic
            ActivateFinalSpace();
            Debug.Log("final space magic again");
            //GameManager.instance.EndTurn();
        }
    }

    //swaps the canvas's camera mode to screen space camera
    public void SwapRenderModeToCamera()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
    }
    
    //swap the canvas's camera mode to screen space overlay
    public void SwapRenderModeToOverlay()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
    
    //function for when player lands on a standard space
    public void ActivateStandardSpaceMenu()
    {
        StartCoroutine(StandardSpaceCoroutine());
    }
    
    //function for when player lands on a flip flop space
    public void ActivateFlipFlopSpace()
    {
        StartCoroutine(FlipFlopSpaceCoroutine());
    }
    
    //function for when player lands or starts on final space
    public void ActivateFinalSpace()
    {
        StartCoroutine(FinalSpaceCoroutine());
    }
    
    //Disable the various space Menus
    public void SetSpaceMenusInactive()
    {
        standardSpaceMenu.SetActive(false);
        wildSpaceMenu.SetActive(false);
       /* allPlaySpaceMenu.SetActive(false);
        doubleDownSpaceMenu.SetActive(false);
        */
       flipFlopSpaceMenu.SetActive(false);
       finalSpaceMenu.SetActive(false);
       timerTextObj.SetActive(false);
       
       for (int i = 0; i < categoryBackgroundImageList.Count; i++)
       {
           categoryBackgroundImageList[i].gameObject.SetActive(false);
       }
    }
    
    //change the flip flop text ui to reflect who's turn it is
    public void AdjustFlipFlopText(int currentFlipFlopTeam)
    {
        flipFlopTeamText.text = "Team " + (currentFlipFlopTeam+1) + " name one...";
    }
    
    //what happens when landing on a flip flop space
    private IEnumerator FlipFlopSpaceCoroutine()
    {
        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        flipFlopSpaceMenu.SetActive(true);
        timerTextObj.SetActive(true);
        
        GameManager.instance.StartFlipFlopTurn();
        
        AdjustFlipFlopText(GameManager.instance.activeFlipFlopPlayerInt);
        
        ChooseCategoryForFlipFlopSpace();

        flipFlopCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(flipLFlopCountdownTime));

    }

    private IEnumerator WildSpaceCoroutine()
    {
        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        ChooseCategoriesForWildSpace();

        for (int i = 0; i < selectWildSpaceEventList.Count; i++)
        {
            selectWildSpaceEventList[i].categoryHasBeenSelected = false;
        }
        
        timerTextObj.SetActive(true);

        wildSpaceText.text = "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + "\nchoose a category";

        wildSpaceCategorySelectionTimerCo = StartCoroutine(NameFiveCountdownCoroutine(categorySelectTime));
    }

    //coroutine for what happens when landing on a regular space
    private IEnumerator StandardSpaceCoroutine()
    {
        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        standardSpaceMenu.SetActive(true);
        timerTextObj.SetActive(true);
        
        ChooseCategoryForStandardSpace();
        
        standardCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(standardCountdownTime));

    }

    //activate category ui for a standard space and pick a random category
    private void ChooseCategoryForStandardSpace()
    {
        categoryManager.RefillCategoryList();
        
        int chosenCategory = categoryManager.PickCategory();
        
        categoryBackgroundImageList[0].gameObject.SetActive(true);
        categoryTextUIList[0].text = categoryManager.categorySOList[chosenCategory].categoryName;
        
        categoryManager.MoveCategoryToUsedCategoryList(chosenCategory);
    }
    
    //activate category UI for flip flop space and pick initial category
    public void ChooseCategoryForFlipFlopSpace()
    {
        categoryManager.RefillCategoryList();

        int chosenCategory = categoryManager.PickCategory();
        
        categoryBackgroundImageList[0].gameObject.SetActive(true);
        categoryTextUIList[0].text = categoryManager.categorySOList[chosenCategory].categoryName;
        
        categoryManager.MoveCategoryToUsedCategoryList(chosenCategory);
    }
    
    //activate category UI for Wild space and pick initial categories for player to choose from
    public void ChooseCategoriesForWildSpace()
    {

        for (int i = 0; i < categoryBackgroundImageList.Count; i++)
        {
            categoryManager.RefillCategoryList();
            
            int chosenCategory = categoryManager.PickCategory();
            
            categoryBackgroundImageList[i].gameObject.SetActive(true);
            categoryTextUIList[i].text = categoryManager.categorySOList[chosenCategory].categoryName;
            
            categoryManager.MoveCategoryToUsedCategoryList(chosenCategory);
        }
    }


    private IEnumerator FinalSpaceCoroutine()
    {
        GameManager.instance.AttemptToWin();

        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        finalSpaceMenu.SetActive(true);
        timerTextObj.SetActive(true);

        standardCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(GameManager.instance.timeToWin));
    }
    
    public IEnumerator NameFiveCountdownCoroutine(float countdownSeconds)
    {
        float counter = countdownSeconds;
        
        UIController.instance.timerText.text = counter.ToString();
        
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            UIController.instance.timerText.text = counter.ToString();

        }

        if (counter <= 0)
        {
            UIController.instance.timerText.text = "Time's Up!!!";
        }

    }

    public void StopAllCountdownCoroutines()
    {
        StopCoroutine(standardCountdownTimerCo);
        StopCoroutine(flipFlopCountdownTimerCo);
        StopCoroutine(wildSpaceCountdownTimerCo);
        StopCoroutine(winTimeCountdownTimerCo);
        StopCoroutine(wildSpaceCategorySelectionTimerCo);
        
    }
    
    

   
}
