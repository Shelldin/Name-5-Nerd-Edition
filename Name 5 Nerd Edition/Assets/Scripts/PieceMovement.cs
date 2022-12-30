using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    public bool isMovePhase;
    public DiceRoller diceRoller;
    public float moveSpeed = 5f;

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
        //piece movement
        if (isMovePhase)
        {
            int activeInt = GameManager.instance.currentPlayerTurnCount - 1;
            GamePieceSO activeSO = GameManager.instance.currentPlayerPieceSOList[activeInt];
            GameObject activePieceObj = GameManager.instance.gamePieceObjList[activeInt];
            
            if (diceRoller.rollResultInt > 0 && activeSO.activePiece && activeSO.spaceNumber < movePositionsList.Count)
            {
                activeSO.nextSpace = movePositionsList[activeSO.spaceNumber + 1];
                activeSO.nextPos = activeSO.nextSpace.transform.position;
                
                if (activePieceObj.transform.position != activeSO.nextPos)
                {
                    activePieceObj.transform.position = Vector3.MoveTowards
                        (activePieceObj.transform.position, activeSO.nextPos,
                            moveSpeed * Time.deltaTime);
                }
            }
                
        }
    }
}
