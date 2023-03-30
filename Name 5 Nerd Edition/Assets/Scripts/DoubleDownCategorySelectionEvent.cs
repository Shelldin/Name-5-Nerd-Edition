using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleDownCategorySelectionEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public CategoryChoiceCounterSO categoryChoiceCountSO;

    public Image categoryImage;
    public TMP_Text categoryText;
    
    public bool categoryHasBeenSelected = false;

    public bool isDoubleDownTurn;
    
    public List<GameObject> categoryGameObjList = new List<GameObject>();


    public void OnPointerUp(PointerEventData eventData)
    {

        if (isDoubleDownTurn)
        {
            if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt < 1)
            {
                categoryHasBeenSelected = true;
                
                //CODE TO CHANGE ALPHA VALUE OF IMAGE AND TEXT

                //UIController.instance.WildCategoryChosen();

            }
        }

        else if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                 == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt == 1)
        {
            for (int i = 0; i < categoryGameObjList.Count; i++)
            {
                if (!categoryHasBeenSelected)
                {
                    categoryGameObjList[i].gameObject.SetActive(false);
                }
            }
            
            categoryHasBeenSelected = true;
        }

        /*else if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                   == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt > 1)
        {
            
        }*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
