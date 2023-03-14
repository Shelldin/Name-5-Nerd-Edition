using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GamePieceSO> gamePieceSOList = new List<GamePieceSO>();
    public List<GamePieceSO> currentPlayerPieceSOList = new List<GamePieceSO>();
    public List<GameObject> gamePieceObjList = new List<GameObject>();
    public List<GamePieceSO> flipFlopGamePieceSOList = new List<GamePieceSO>();

    public PieceMovement pieceMovement;
    public CameraController cameraCon;

    private Scene currentScene;
    public String currentSceneName;

    public int currentPlayerTurnCount;

    public int numberOfDiceRollsThisTurn = 0;

    private WaitForSeconds movePhaseWFS;
    public float movePhaseDelay = 1f;

    private WaitForSeconds countdownWFS;
    public float timeToWin = 90f;

    public int activeFlipFlopPlayerInt,
        flipFlopLosersInt = 0;

    public CategoryManager categoryManager;
    
        
    

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

        pieceMovement = gameObject.GetComponent(typeof(PieceMovement)) as PieceMovement;

        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        movePhaseWFS = new WaitForSeconds(movePhaseDelay);

        currentPlayerTurnCount = 0;

        
       // winTimeCountdownTimerCo = StartCoroutine(NameFiveCountdownCoroutine(timeToWin));

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //calls when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //get all the move points and sort them
        pieceMovement.PopulateMovePositionsList();
        
        //if main game scene loads populate list of current players. else clear same list.
        PopulateOrClearPlayerList();
        
        //set the Render Camera to the main camera of the current scene
        UIController.instance.SetRenderCamera();
        
        
        
        //start the game if current scene is the main game scene
        if (currentSceneName == "MainGameScene")
        {
          StartGame();

          cameraCon = FindObjectOfType<CameraController>();
          
          cameraCon.SetMoveTargetToCurrentPlayer();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //load the start menu scene
    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene("StartMenuScene");
    }

    //load the main game scene
    public void LoadMainGameScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    //populates the currentPlayerList or clears it depending on the current scene.
    private void PopulateOrClearPlayerList()
    {
        //check to see which scene is currently active
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        //if the current scene is the main game scene, populate the current player list based on playerCountInt.
        if (currentSceneName == "MainGameScene")
        {
            for (int i = 0; i < playerCountInt; i++)
            {
                currentPlayerPieceSOList.Add(gamePieceSOList[i]);
            }
        }
        //if active scene is not the main game scene clear the list of current players.
        else
        {
            currentPlayerPieceSOList.Clear();
        }

        currentPlayerPieceSOList.Sort(delegate(GamePieceSO i1, GamePieceSO i2)
        {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
    }
    
    //start of team/player turn
    public void BeginPlayerTurn()
    {
        UIController.instance.StartPlayerTurn();
    }
    
    //take another roll
    public void RollDiceAgain()
    {
        if (numberOfDiceRollsThisTurn < 5)
        {
            UIController.instance.SwapRenderModeToOverlay();
            UIController.instance.SetDiceMenuActive();
            UIController.instance.diceButton.interactable = true;
            UIController.instance.SetSpaceMenusInactive();
        }
        else
        {
            //end turn function
        }
    }

    //begin the game after finishing setup in main menu
    private void StartGame()
    {
        PrepareAllPlayerPieces();
        BeginPlayerTurn();
        currentPlayerPieceSOList[currentPlayerTurnCount].activePiece = true;
    }

    //instantiate the players at the beginning of the game with proper color and turn order
    private void PrepareAllPlayerPieces()
    {
        
        //instantiate each player according to how many teams will be playing
        for (int i = 0; i < currentPlayerPieceSOList.Count; i++)
        {
            //reset the current spaces to 0 (start space) when instantiated
            currentPlayerPieceSOList[i].spaceNumber = 0;
            
            //reset the next piece and next position
            currentPlayerPieceSOList[i].nextSpace = pieceMovement.movePositionsList[0];
            currentPlayerPieceSOList[i].nextPos = pieceMovement.movePositionsList[0].transform.position;
            
            //set the current position to the start space of the game board
            currentPlayerPieceSOList[i].currentPos = pieceMovement.movePositionsList[0].gameObject.transform.position;
            currentPlayerPieceSOList[i].currentSpace = pieceMovement.movePositionsList[0];
            
            //reset all onFinalPiece bools to false
            currentPlayerPieceSOList[i].onFinalSpace = false;

            //change player name for sorting later
            currentPlayerPieceSOList[i].pieceObj.name = "Player" + i;

            currentPlayerPieceSOList[i].categoriesToWinInt = 6;

            //set spawn point with an offset so player pieces are slightly spread and don't overlap;
            Vector3 spawnPoint =
                new Vector3(currentPlayerPieceSOList[i].currentPos.x + currentPlayerPieceSOList[i].offset.x,
                    currentPlayerPieceSOList[i].currentPos.y + currentPlayerPieceSOList[i].offset.y, 0);

            //instantiation
            Instantiate(currentPlayerPieceSOList[i].pieceObj, spawnPoint, Quaternion.identity);
        }
        
        //add the newly spawned pieces to list of game objects and sort it
        gamePieceObjList.AddRange(GameObject.FindGameObjectsWithTag("PlayerPiece"));
        gamePieceObjList.Sort(delegate(GameObject go1, GameObject go2)
        {return String.Compare(go1.name, go2.name, StringComparison.Ordinal);});

        
        //A probably way-more-complicated-than-needed way to change the instantiated game pieces to the correct color.
        List<SpriteRenderer> gamePieceSpriteList = new List<SpriteRenderer>();
        
        for (int i = 0; i < gamePieceObjList.Count; i++)
        {
             SpriteRenderer hasSpriteRenderer = gamePieceObjList[i].GetComponent<SpriteRenderer>();

            if (hasSpriteRenderer)
            {
                gamePieceSpriteList.Add(hasSpriteRenderer);
            }

            gamePieceSpriteList[i].color = currentPlayerPieceSOList[i].gamePieceColor;
        }
    }
    
    //track number of times the dice has been rolled
    public void IncreaseDiceRollCount()
    {
        numberOfDiceRollsThisTurn++;
    }

    //set the dice roll tracker back to 0
    public void ResetDiceRollCount()
    {
        numberOfDiceRollsThisTurn = 0;
    }

    public void ActivateMovePhase()
    {
        StartCoroutine(ActivateMovePhaseCoroutine());
    }
    
    //activate the move phase after a short delay
    public IEnumerator ActivateMovePhaseCoroutine()
    {
        yield return movePhaseWFS;

        pieceMovement.isMovePhase = true;
    }
    
    //swap to next player at the end of the turn
    public void EndTurn()
    {
        //advance the turn to next player
        currentPlayerPieceSOList[currentPlayerTurnCount].activePiece = false;
        
        //reset dice related ints to 0 for next player
        pieceMovement.diceRoller.rollResultInt = 0;
        numberOfDiceRollsThisTurn = 0;

        pieceMovement.isMovePhase = false;
        
        //make sure playerTurnCount doesnt exceed total number of players
        if (currentPlayerTurnCount < currentPlayerPieceSOList.Count - 1)
        {
            currentPlayerTurnCount++;
        }
        else if (currentPlayerTurnCount >= currentPlayerPieceSOList.Count - 1)
        {
            currentPlayerTurnCount = 0;
        }
        
        //set next player piece as active
        currentPlayerPieceSOList[currentPlayerTurnCount].activePiece = true;
        
        UIController.instance.StartPlayerTurn();
    }

    //method for what happens when player takes a turn after landing on the final space
    public void AttemptToWin()
    {
        
        int numberOfCategoriesToWin;

        if (currentPlayerPieceSOList[currentPlayerTurnCount].onFinalSpace)
        {
            //dont let the number of categories to win drop below 1
            if (currentPlayerPieceSOList[currentPlayerTurnCount].categoriesToWinInt <= 1)
            {
                currentPlayerPieceSOList[currentPlayerTurnCount].categoriesToWinInt = 1;
            }
            //decrease the number of categories a player needs to win by 1 each time they fail to win (until they only have 1)
            else
            {
                currentPlayerPieceSOList[currentPlayerTurnCount].categoriesToWinInt--;
            }
            
            numberOfCategoriesToWin = currentPlayerPieceSOList[currentPlayerTurnCount].categoriesToWinInt;
            Debug.Log("categories = " + numberOfCategoriesToWin);
           
            
            if (numberOfCategoriesToWin > 5)
            {
                numberOfCategoriesToWin = 5;
            }

            if (numberOfCategoriesToWin is >= 4)
            {
                timeToWin = 90f;
            }

            else if (numberOfCategoriesToWin == 3)
            {
                timeToWin = 60f;
            }
            else if (numberOfCategoriesToWin is >= 1 and < 3)
            {
                timeToWin = 30f;
            }
            else
            {
                Debug.Log("less than 1");
                numberOfCategoriesToWin = 1;
                timeToWin = 30f;
            }
        }
        
    }

    //fill the flipflopgamepieceSOlist with the current players
    private void FillFlipFlopGamePieceSOList()
    {
        //add current players to the flipflop list
        for (int i = 0; i < currentPlayerPieceSOList.Count; i++)
        {
            flipFlopGamePieceSOList.Add(currentPlayerPieceSOList[i]);
        }
        
        //sort the players in the flip flop list
        flipFlopGamePieceSOList.Sort(delegate(GamePieceSO i1, GamePieceSO i2)
            {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});

        //make sure each player is have a chance to compete for the flip flop round
        foreach (GamePieceSO gamePieceSo in flipFlopGamePieceSOList)
        {
            gamePieceSo.hasLostCurrentFlipFlop = false;
        }
        
    }

    public void StartFlipFlopTurn()
    {
        activeFlipFlopPlayerInt = currentPlayerTurnCount;

        flipFlopLosersInt = 0;

        FillFlipFlopGamePieceSOList();
        
    }
    
    //reset the flip flop list at the end of the flip flop turn
    private void EmptyFlipFlopList()
    {
        for (int i = 0; i < flipFlopGamePieceSOList.Count; i++)
        {
            flipFlopGamePieceSOList.Remove(flipFlopGamePieceSOList[i]);  
        }
        
    }

    //when pressing success button during flip flop: select the next player who hasn't failed and reset timer
    public void FlipFLopSuccess()
    {
        int exitCounter = 5;
        
        SelectNextFlipFlopPlayer();
        while (flipFlopGamePieceSOList[activeFlipFlopPlayerInt].hasLostCurrentFlipFlop && exitCounter > 0)
        {
            exitCounter--;
            SelectNextFlipFlopPlayer();
            
        Debug.Log(exitCounter);
        Debug.Log(activeFlipFlopPlayerInt);
            
        }
        
        UIController.instance.StopAllCountdownCoroutines();

        UIController.instance.flipFlopCountdownTimerCo = StartCoroutine(
            UIController.instance.NameFiveCountdownCoroutine(UIController.instance.flipLFlopCountdownTime));

        UIController.instance.AdjustFlipFlopText(activeFlipFlopPlayerInt);
        UIController.instance.ChooseCategoryForFlipFlopSpace();
        
    }

    public void flipFlopFailure()
    {
        flipFlopGamePieceSOList[activeFlipFlopPlayerInt].hasLostCurrentFlipFlop = true;

        flipFlopLosersInt++;

        int exitCounter = 5;
        
        SelectNextFlipFlopPlayer();
        while (flipFlopGamePieceSOList[activeFlipFlopPlayerInt].hasLostCurrentFlipFlop && exitCounter > 0)
        {
            exitCounter--;
            SelectNextFlipFlopPlayer();
            
            Debug.Log(exitCounter);
            Debug.Log(activeFlipFlopPlayerInt);
            
        }

        if (flipFlopLosersInt >= flipFlopGamePieceSOList.Count-1)
        {
            //flip flop win method
            flipFlopLosersInt = 0;
        }
        
        UIController.instance.StopAllCountdownCoroutines();

        UIController.instance.flipFlopCountdownTimerCo = StartCoroutine(
            UIController.instance.NameFiveCountdownCoroutine(UIController.instance.flipLFlopCountdownTime));

        UIController.instance.AdjustFlipFlopText(activeFlipFlopPlayerInt);
        UIController.instance.ChooseCategoryForFlipFlopSpace();
        
    }

    public void SelectNextFlipFlopPlayer()
    {
        activeFlipFlopPlayerInt++;
        Debug.Log(activeFlipFlopPlayerInt);
        if (activeFlipFlopPlayerInt >= flipFlopGamePieceSOList.Count)
        {
            activeFlipFlopPlayerInt = 0;
            Debug.Log("resetting to " + activeFlipFlopPlayerInt);
        } 
        
        
    }

    private void FlipFlopEndTurn()
    {
        if (activeFlipFlopPlayerInt == currentPlayerTurnCount)
        {
            //resume turn code goes here
        }
        else
        {
            //advance the turn to next player
            currentPlayerPieceSOList[currentPlayerTurnCount].activePiece = false;
        
            //reset dice related ints to 0 for next player
            pieceMovement.diceRoller.rollResultInt = 0;
            numberOfDiceRollsThisTurn = 0;

            pieceMovement.isMovePhase = false;
        
            //make the flip flop winner the new active player
            currentPlayerTurnCount = activeFlipFlopPlayerInt;
            
            //empty flip flop list
            EmptyFlipFlopList();
        
            //set next player piece as active
            currentPlayerPieceSOList[currentPlayerTurnCount].activePiece = true;

            UIController.instance.StartPlayerTurn();
        }
    }

}
