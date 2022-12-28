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

    public PieceMovement pieceMovement;

    private Scene currentScene;
    private String currentSceneName;
    



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
}
