using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GamePieceSO")]

public class GamePieceSO : ScriptableObject
{
   public GameObject pieceObj;
   public SpriteRenderer pieceSprite;
   public Color gamePieceColor;

   public bool activePiece = false;
   
   public bool onFinalSpace = false;
   public int categoriesToWinInt = 6;
   
   public Vector3 currentPos;
   public Vector3 nextPos;
   
   public GameObject currentSpace;
   public GameObject nextSpace;

   //the number of the space according to its spot in in the movePositionList
   public int spaceNumber = 0;

   //used to make so the pieces are visible when it's not their turn and aren't all stacked on each other
   public Vector3 offset;

   //used the change the color of the game piece sprites
   public void SetPieceColor(Color color)
   {
      pieceSprite.color = color;
      gamePieceColor = pieceSprite.color;
   }
   
   //used to determine which game piece is currently taking its turn
   public void SetActivePiece()
   {
      activePiece = !activePiece;
   }

   //used to update a piece's position on the board
   public void ChangeCurrentPositionToNextPosition()
   {
      currentPos = nextPos;
   }
}
