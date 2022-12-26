using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GamePieceSO")]

public class GamePieceSO : ScriptableObject
{
   public SpriteRenderer pieceSprite;

   public bool activePiece = false;
   
   public Vector2 currentPos;
   public Vector2 nextPos;

   public void SetActivePiece()
   {
      activePiece = !activePiece;
   }

   public void ChangeCurrentPositionToNextPosition()
   {
      currentPos = nextPos;
   }
}
