using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
   public float rollAnimationTime;
   
   private IEnumerator DiceRollCoroutine()
   {
      yield return new WaitForSeconds(rollAnimationTime);
   }

   public void RollDiceButton()
   {
      StartCoroutine(DiceRollCoroutine());
   }
}
