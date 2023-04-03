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
                 diceRoller.rollResultInt + activeSO.spaceNumber +1 <= movePositionsList.Count
                 && !activeSO.onFinalSpace)
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
                        /*if the piece land on the final space set onFinalSpace to true
                        and begin special endgame mechanics*/
                        if (activeSO.spaceNumber + 1 == movePositionsList.Count)
                        {
                            activeSO.onFinalSpace = true;
                            //let player try to win the game when on the final space
                            
                            UIController.instance.ActivateFinalSpace();
                            Debug.Log("Final Space magic should be happening now");
                            
                            // xxx remove when endgame method is added 
                            //GameManager.instance.EndTurn();
                            // xxx
                        }
                        //if not on final space on board continue turns as normal
                        else
                        {
                            if (GameManager.instance.numberOfDiceRollsThisTurn < 5)
                            {

                                if (activeSO.currentSpace.layer == LayerMask.NameToLayer("StandardSpace"))
                                {
                                    Debug.Log("This is a standard space");
                                    UIController.instance.ActivateStandardSpaceMenu();
                                }
                                else if (activeSO.currentSpace.layer == LayerMask.NameToLayer("FlipFlopSpace"))
                                {
                                    Debug.Log("this is a Flip Flop space");
                                    
                                    UIController.instance.ActivateFlipFlopSpace();
                                }
                                else if (activeSO.currentSpace.layer == LayerMask.NameToLayer("WildSpace"))
                                {
                                    Debug.Log("This is a Wild Space");
                                    UIController.instance.ActivateWildSpace();
                                }
                                else if (activeSO.currentSpace.layer == LayerMask.NameToLayer("DoubleDownSpace"))
                                {
                                    UIController.instance.ActivateDoubleDownSpace();
                                }
                                else
                                {
                                    Debug.Log("this is NOT a standard space");
                                    //XXX CHANGE TO OTHER SPACE TYPES XXX
                                    UIController.instance.ActivateStandardSpaceMenu();
                                }
                                
                            }
                            else
                            {
                                GameManager.instance.EndTurn();
                            }
                        }
                    }
                }
            }
            /*if (at the end of the board) the piece rolls great than the remaining spaces on the board,
             have roll attempts till out of rolls or roll the exact amount of spaces needed*/
            else if (diceRoller.rollResultInt > 0 && activeSO.activePiece &&
                     diceRoller.rollResultInt + activeSO.spaceNumber +1 > movePositionsList.Count &&
                     !activeSO.onFinalSpace)
            {
                isMovePhase = false;
                /*if player hasn't rolled more than 5 times,
                 continue to roll til they make it to final space or roll 5 times*/
                if (GameManager.instance.numberOfDiceRollsThisTurn < 5)
                {
                    //card method
                    UIController.instance.ActivateStandardSpaceMenu();
                }
                //if player has rolled 5 times end turn
                else
                {
                    GameManager.instance.EndTurn();
                }
                
            }
        }
    }

    private IEnumerator FinalSpaceRepeatRollCoroutine()
    {
        
        yield return wfs;
        
        
    }
}
