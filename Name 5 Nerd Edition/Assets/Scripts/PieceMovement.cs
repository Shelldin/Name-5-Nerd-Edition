using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{

    public List<GameObject> movePositionsList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        movePositionsList.AddRange(GameObject.FindGameObjectsWithTag("MovePoint"));
        
        movePositionsList.Sort();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
