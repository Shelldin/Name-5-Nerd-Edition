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
        difficultyMenu,
        diceRollMenu,
        standardSpaceMenu,
        allPlaySpaceMenu,
        flipFlopSpaceMenu,
        wildSpaceMenu,
        doubleDownSpaceMenu,
        finalSpaceMenu,
        timerTextObj,
        instructionObj,
        allPlayInstructionObj,
        victoryUIObj;

    public float standardCountdownTime = 30f,
        flipLFlopCountdownTime = 10f,
        wildSpaceCountdownTime = 30f,
        doubleDownCountdownTime = 30f,
        categorySelectTime = 15f;

    public Button diceButton;

    public Button allPlayCurrentTeamButton,
        allPlayOpposingTeamButton;

    public Canvas canvas;

    public TMP_Text colorInstructionText,
        playerTurnText,
        flipFlopTeamText,
        wildSpaceText,
        doubleDownText,
        instructionText,
        allPlayInstructionText,
        timerText,
        allPlayCurrentTeamText,
        allPlayOpposingTeamText,
        victoryText;

    //public List<Image> colorButtonImagesList = new List<Image>();

    public Image colorButtonImage;

    private int playerColorChoiceCoutdown;

    private WaitForSeconds wfs;
    private WaitForSeconds allPlayWFS;
    public float timeDelay = 3f;
    public float allPlayInstructionDelay = 5f;

    public Coroutine standardCountdownTimerCo,
        flipFlopCountdownTimerCo,
        wildSpaceCountdownTimerCo,
        doubleDownCountdownTimerCo,
        winTimeCountdownTimerCo,
        CategorySelectionTimerCo;

    public CategoryManager categoryManager;
    public List<SelectWildSpaceCategoryEvent> selectWildSpaceEventList = new List<SelectWildSpaceCategoryEvent>();
    public List<DoubleDownCategorySelectionEvent> selectDoubleDownEventList = new List<DoubleDownCategorySelectionEvent>();
    public List<Image> categoryBackgroundImageList = new List<Image>();
    public List<TMP_Text> categoryTextUIList = new List<TMP_Text>();

    public CategoryChoiceCounterSO catChoiceCounterSO;


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
        allPlayWFS = new WaitForSeconds(allPlayInstructionDelay);
        
        standardCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(standardCountdownTime));
        flipFlopCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(flipLFlopCountdownTime));
        wildSpaceCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(wildSpaceCountdownTime));
        doubleDownCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(doubleDownCountdownTime));
        winTimeCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(GameManager.instance.timeToWin));
        CategorySelectionTimerCo = StartCoroutine(NameFiveCountdownCoroutine(categorySelectTime));

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
            difficultyMenu.SetActive(true);
           // GameManager.instance.LoadMainGameScene();
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
    
    //function for when player lands on wild space
    public void ActivateWildSpace()
    {
        StartCoroutine(WildSpaceCoroutine());
    }

    public void ActivateDoubleDownSpace()
    {
        StartCoroutine(DoubleDownCoroutine());
    }

    public void ActivateAllPlaySpace()
    {
        StartCoroutine(AllPlayCoroutine());
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
        allPlaySpaceMenu.SetActive(false);
        doubleDownSpaceMenu.SetActive(false);
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
        instructionObj.SetActive(true);
        

        instructionText.text = "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + "choose a category";

        CategorySelectionTimerCo = StartCoroutine(NameFiveCountdownCoroutine(categorySelectTime));
    }
    

    //Coroutine to start double down turn
    private IEnumerator DoubleDownCoroutine()
    {
        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        ChooseCategoriesForDoubleDownSpace();

        for (int i = 0; i < selectDoubleDownEventList.Count; i++)
        {
            selectDoubleDownEventList[i].categoryHasBeenSelected = false;
        }
        
        timerTextObj.SetActive(true);
       instructionObj.SetActive(true);
       
       catChoiceCounterSO.ResetCategoriesChosenInt();

        instructionText.text = "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + "choose two categories";

        CategorySelectionTimerCo = StartCoroutine(NameFiveCountdownCoroutine(categorySelectTime));
    }

    private IEnumerator AllPlayCoroutine()
    {
        yield return wfs;
        SwapRenderModeToOverlay();
        
        StopAllCountdownCoroutines();
        
        
        ChooseCategoriesForAllPlaySpace();
        
        allPlayInstructionObj.SetActive(true);

        int currentPlayerInt = GameManager.instance.currentPlayerTurnCount;
        int opposingPlayerInt;

        if (GameManager.instance.currentPlayerTurnCount == GameManager.instance.currentPlayerPieceSOList.Count - 1)
        {
            opposingPlayerInt = 0;
        }
        else
        {
            opposingPlayerInt = GameManager.instance.currentPlayerTurnCount + 1;
        }

        allPlayInstructionText.text = "Team " + (currentPlayerInt + 1) + " and Team " + (opposingPlayerInt + 1) +
                               "\n Name 5 of your Category before the other team.";

        yield return allPlayWFS;

        allPlayCurrentTeamText.text = "Team " + (currentPlayerInt + 1);
        allPlayOpposingTeamText.text = "Team " + (opposingPlayerInt + 1);
        
        allPlayInstructionObj.SetActive(false);
        
        allPlaySpaceMenu.SetActive(true);
        
        categoryBackgroundImageList[3].gameObject.SetActive(true);
        categoryBackgroundImageList[4].gameObject.SetActive(true);

        allPlayCurrentTeamButton.image.color =
            GameManager.instance.currentPlayerPieceSOList[currentPlayerInt].gamePieceColor;
        allPlayOpposingTeamButton.image.color =
            GameManager.instance.currentPlayerPieceSOList[opposingPlayerInt].gamePieceColor;

    }

    //activate main wild space menu after player has selected 1 out of the 5 categories they wish to name. Called in SelectWildSpaceCategoryEvent
    public void WildCategoryChosen()
    {
        instructionObj.SetActive(false);
        
        wildSpaceMenu.SetActive(true);
            

        wildSpaceText.text =
            "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + " name 5...";
            
        StopAllCountdownCoroutines();

        wildSpaceCountdownTimerCo = 
            StartCoroutine(NameFiveCountdownCoroutine(wildSpaceCountdownTime));
        
    }

    //acitvate double down space menu after 2 categories have been selected. called in DoubleDownCategorySelectionEvent
    public void DoubleDownCategoriesChosen()
    {
       instructionObj.SetActive(false);
        
        doubleDownSpaceMenu.SetActive(true);

        for (int i = 0; i < categoryBackgroundImageList.Count; i++)
        {
            ResetCategoryImageAlpha(categoryBackgroundImageList[i], categoryTextUIList[i]);
        }

        doubleDownText.text = "Team " + (GameManager.instance.currentPlayerTurnCount + 1) + " name 5...";
        
        StopAllCountdownCoroutines();

        doubleDownCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(doubleDownCountdownTime));
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

    public void ChooseCategoriesForDoubleDownSpace()
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

    public void ChooseCategoriesForAllPlaySpace()
    {
        int categoriesToChooseInt = 2;

        while (categoriesToChooseInt > 0)
        {
            categoryManager.RefillCategoryList();
        
            int chosenCategory = categoryManager.PickCategory();
            
            categoryTextUIList[5 - categoriesToChooseInt].text = categoryManager.categorySOList[chosenCategory].categoryName;
        
            categoryManager.MoveCategoryToUsedCategoryList(chosenCategory);

            categoriesToChooseInt--;
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
        
        timerText.text = counter.ToString();
        
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            timerText.text = counter.ToString();

        }

        if (counter <= 0)
        {
            instance.timerText.text = "Time's Up!!!";
        }

    }

    public void StopAllCountdownCoroutines()
    {
        StopCoroutine(standardCountdownTimerCo);
        StopCoroutine(flipFlopCountdownTimerCo);
        StopCoroutine(wildSpaceCountdownTimerCo);
        StopCoroutine(doubleDownCountdownTimerCo);
        StopCoroutine(winTimeCountdownTimerCo);
        StopCoroutine(CategorySelectionTimerCo);
        
    }

    private void ResetCategoryImageAlpha(Image image, TMP_Text text)
    {
        Color categoryImageColor = image.color;
        Color categoryTextColor = text.color;
        

        categoryImageColor.a = 1f;
        categoryTextColor.a = 1f;

        image.color = categoryImageColor;
        text.color = categoryTextColor;
    }

    public void SetVictoryUI()
    {
        SetSpaceMenusInactive();
        victoryUIObj.SetActive(true);
        victoryText.text = "Congratulations Team " + GameManager.instance.currentPlayerTurnCount + 1 + "!!! You are the most"
                           + " intelligent and beautiful team of all time. \n All other players are uneducated fart-sacks.";
    }

   
}
