using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GamePieceSO> gamePieceSOList = new List<GamePieceSO>();
    public List<GamePieceSO> currentPlayerPieceSOList = new List<GamePieceSO>();
    public List<GameObject> gamePieceObjList = new List<GameObject>();

    public PieceMovement pieceMovement;
    public CameraController cameraCon;

    private Scene currentScene;
    public String currentSceneName;

    public int currentPlayerTurnCount;

    public int numberOfDiceRollsThisTurn = 0;

    private WaitForSeconds wfs;
    public float movePhaseDelay = 1f;
    



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

        wfs = new WaitForSeconds(movePhaseDelay);

        currentPlayerTurnCount = 1;
        

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
                Debug.Log(i);
            }
        }
        //if active scene is not the main game scene clear the list of current players.
        else
        {
            Debug.Log("else statement");
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
        currentPlayerPieceSOList[currentPlayerTurnCount -1].activePiece = true;
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

            //change player name for sorting later
            currentPlayerPieceSOList[i].pieceObj.name = "Player" + i;

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
        yield return wfs;

        pieceMovement.isMovePhase = true;
    }

}
