using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{

    public List<GameObject> movePositionsList = new List<GameObject>();

    //add all move points to a list and sort them in order they appear on the board
    private void PopulateMovePositionsList()
    {
        
        movePositionsList.AddRange(GameObject.FindGameObjectsWithTag("MovePoint"));
        
        movePositionsList.Sort();
    }

    void Update()
    {
        
    }
}
