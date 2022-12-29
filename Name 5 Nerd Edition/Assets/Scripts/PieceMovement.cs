using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    public bool isMovePhase;
    public DiceRoller diceRoller;

    public List<GameObject> movePositionsList = new List<GameObject>();

    //add all move points to a list and sort them in order they appear on the board
    public void PopulateMovePositionsList()
    {
        Debug.Log("move piece list thing");
        movePositionsList.AddRange(GameObject.FindGameObjectsWithTag("MovePoint"));

        movePositionsList.Sort(delegate(GameObject i1, GameObject i2)
        {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
    }

    private void Update()
    {
        if (isMovePhase)
        {
            if (diceRoller.rollResultInt > 0 && 
                GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].activePiece)
            {
                
            }
                
        }
    }
}
