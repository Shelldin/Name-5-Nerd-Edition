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
   
   public Vector3 currentPos;
   public Vector3 nextPos;

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
