using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoubleDownCategorySelectionEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public CategoryChoiceCounterSO categoryChoiceCountSO;

    public Image categoryImage;
    public TMP_Text categoryText;
    
    public bool categoryHasBeenSelected = false;

    public bool isDoubleDownTurn;
    
    public List<DoubleDownCategorySelectionEvent> categoryGameObjList = new List<DoubleDownCategorySelectionEvent>();


    

    public void OnPointerUp(PointerEventData eventData)
    {

        
        //check for on double down space & category selected is false & category counter SO is <1
        if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt < 1)
        {
            categoryHasBeenSelected = true;
            
            ReduceImageAndTextAlpha();
            
            categoryChoiceCountSO.IncreaseCategoriesChosenCounterInt();

        }
        

        else if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                 == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt == 1)
        {
            for (int i = 0; i < categoryGameObjList.Count; i++)
            {
                if (!categoryGameObjList[i].categoryHasBeenSelected)
                {
                    categoryGameObjList[i].gameObject.SetActive(false);
                }
            }
            
            categoryHasBeenSelected = true;
            
            categoryChoiceCountSO.IncreaseCategoriesChosenCounterInt();
            
            UIController.instance.DoubleDownCategoriesChosen();
        }

        /*else if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer
                   == LayerMask.NameToLayer("DoubleDownSpace") && !categoryHasBeenSelected && categoryChoiceCountSO.categoriesChosenInt > 1)
        {
            
        }*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    //reduce the alpha to signify the category has been chosen
    public void ReduceImageAndTextAlpha()
    {
        Color categoryTextColor = categoryText.color;
        Color categoryImageColor = categoryImage.color;

        categoryImageColor.a = .5f;
        categoryTextColor.a = .5f;

        categoryImage.color = categoryImageColor;
        categoryText.color = categoryTextColor;

    }
    
}
