using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
   public float rollAnimationTime = 3f;
   private int rollResultInt;

   private int minRoll = 1,
      maxRoll = 6;

   public Animator anim;

   private IEnumerator DiceRollCoroutine()
   {
      //reset roll result so rolling animation functions properly
      rollResultInt = 0;
      anim.SetInteger("RollResultInt", rollResultInt);
      
      //dice rolling animation
      anim.SetTrigger("DiceRollButtonPress");
      
      yield return new WaitForSeconds(rollAnimationTime);

      //randomly select which number the dice "lands" on and reflect that in the animation.
      rollResultInt = Random.Range(minRoll, maxRoll);
      anim.SetInteger("RollResultInt", rollResultInt);
   }

   public void RollDiceButton()
   {
      StartCoroutine(DiceRollCoroutine());
   }
}
