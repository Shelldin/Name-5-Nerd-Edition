using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectWildSpaceCategoryEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool categoryHasBeenSelected = false;

    public bool isWildSpaceTurn;
    
    public List<GameObject> categoryGameObjList = new List<GameObject>();


    public void OnPointerUp(PointerEventData eventData)
    {

        
        
        if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer == LayerMask.NameToLayer("WildSpace") 
                && !categoryHasBeenSelected)
        {
            for (int i = 0; i < categoryGameObjList.Count; i++)
            { 
                if (!categoryHasBeenSelected)
                {
                    categoryGameObjList[i].gameObject.SetActive(false);
                } 
            }
            categoryHasBeenSelected = true;
            
            UIController.instance.WildCategoryChosen();

        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
    
}
