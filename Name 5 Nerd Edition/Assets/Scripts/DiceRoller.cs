using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRoller : MonoBehaviour
{
   public float rollAnimationTime = 3f;
   
   //[HideInInspector]
   public int rollResultInt;

   private WaitForSeconds wfs;

   public int minRoll = 1,
      maxRoll = 6;

   private void Start()
   {
      wfs = new WaitForSeconds(rollAnimationTime);
   }

   public Animator anim;

   private IEnumerator DiceRollCoroutine()
   {
      //reset roll result so rolling animation functions properly
      rollResultInt = 0;
      anim.SetInteger("RollResultInt", rollResultInt);
      
      //dice rolling animation
      anim.SetTrigger("DiceRollButtonPress");
      
      yield return wfs;

      //randomly select which number the dice "lands" on and reflect that in the animation.
      rollResultInt = Random.Range(minRoll, maxRoll);
      anim.SetInteger("RollResultInt", rollResultInt);

      //double the result after a successful double down turn
      if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
          == LayerMask.NameToLayer("DoubleDownSpace") && GameManager.instance.numberOfDiceRollsThisTurn >= 2)
      {
         rollResultInt += rollResultInt;
      }
      
      yield return wfs;
      
      //set dice menu inactive after seeing dice result
      UIController.instance.SetDiceMenuInactive();
      UIController.instance.SwapRenderModeToCamera();
   }
   
   

   public void RollDiceButton()
   {
      StartCoroutine(DiceRollCoroutine());
   }
}
