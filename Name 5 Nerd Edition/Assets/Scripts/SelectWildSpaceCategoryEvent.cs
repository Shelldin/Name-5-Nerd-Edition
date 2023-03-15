using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectWildSpaceCategoryEvent : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public List<GameObject> categoryGameObjList = new List<GameObject>();


    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.currentPlayerPieceSOList[GameManager.instance.currentPlayerTurnCount].currentSpace.layer == LayerMask.NameToLayer("WildSpace"))
        {
            for (int i = 0; i < categoryGameObjList.Count; i++)
            {
                categoryGameObjList[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
