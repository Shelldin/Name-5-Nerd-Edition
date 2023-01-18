using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    public bool isMovePhase;
    public DiceRoller diceRoller;
    public float moveSpeed = 5f;

    public float waitTime = 1f;

    private WaitForSeconds wfs;

    public List<GameObject> movePositionsList = new List<GameObject>();

    private void Start()
    {
        wfs = new WaitForSeconds(waitTime);
    }

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
            //setting some variables for short coding
            int activeInt = GameManager.instance.currentPlayerTurnCount;
            GamePieceSO activeSO = GameManager.instance.currentPlayerPieceSOList[activeInt];
            GameObject activePieceObj = GameManager.instance.gamePieceObjList[activeInt];
            
            if (diceRoller.rollResultInt > 0 && activeSO.activePiece &&
                 diceRoller.rollResultInt + activeSO.spaceNumber +1 <= movePositionsList.Count)
            {
                //use the movePositionList to determine which space we will move to next
                activeSO.nextSpace = movePositionsList[activeSO.spaceNumber + 1];
                activeSO.nextPos = activeSO.nextSpace.transform.position;
                
                //behold... movement
                if (activePieceObj.transform.position != activeSO.nextPos)
                {
                    activePieceObj.transform.position = Vector3.MoveTowards
                        (activePieceObj.transform.position, activeSO.nextPos,
                            moveSpeed * Time.deltaTime);
                }

                //based on the dice roll result see if we will move to the next space or not.
                if (activePieceObj.transform.position == activeSO.nextPos)
                {
                    diceRoller.rollResultInt--;
                    activeSO.currentPos = activePieceObj.transform.position;
                    activeSO.currentSpace = activeSO.nextSpace;
                    activeSO.spaceNumber++;
                    if (diceRoller.rollResultInt <= 0)
                    {
                        isMovePhase = false;
                        if (GameManager.instance.numberOfDiceRollsThisTurn < 5)
                        {
                            //card method
                            UIController.instance.ActivateStandardSpaceMenu();
                        }
                        else
                        {
                            GameManager.instance.EndTurn();
                        }
                    }
                }
            }
            /*if (at the end of the board) the piece rolls great than the remaining spaces on the board,
             have roll attempts till out of rolls or roll the exact amount of spaces needed*/
            else if (diceRoller.rollResultInt > 0 && activeSO.activePiece &&
                     diceRoller.rollResultInt + activeSO.spaceNumber +1 > movePositionsList.Count)
            {
                if (GameManager.instance.numberOfDiceRollsThisTurn < 5)
                {
                    //card method
                    UIController.instance.ActivateStandardSpaceMenu();
                }
                
            }
            //PROBABLY ALSO A SUPER COOL ELSE IF STATEMENT FOR WHEN ON THE FINAL SPACE? MAYBE NOT THOUGH.
            //WINNER STATE
                
        }
    }

    private IEnumerator FinalSpaceRepeatRollCoroutine()
    {
        
        yield return wfs;
        
        
    }
}
