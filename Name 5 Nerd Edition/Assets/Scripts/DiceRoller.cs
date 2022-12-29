using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRoller : MonoBehaviour
{
   public float rollAnimationTime = 3f;
   private int rollResultInt;

   private WaitForSeconds wfs;

   private int minRoll = 1,
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

      yield return wfs;
      
      //set dice menu inactive after seeing dice result
      UIController.instance.ChangeDiceMenuActiveState();
   }

   public void RollDiceButton()
   {
      StartCoroutine(DiceRollCoroutine());
   }
}
