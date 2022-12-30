using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 moveTarget;
    public float moveSpeed = 30f;
    
    void Awake()
    {
        moveTarget = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SetMoveTargetToCurrentPlayer();
        if (moveTarget != transform.position)
        {
            transform.position = Vector3.MoveTowards
                (transform.position, moveTarget, moveSpeed * Time.deltaTime);
        }
    }

    public void SetMoveTargetToCurrentPlayer()
    {
        moveTarget = new  Vector3 (GameManager.instance.gamePieceObjList
                [GameManager.instance.currentPlayerTurnCount - 1].transform.position.x,
            GameManager.instance.gamePieceObjList[GameManager.instance.currentPlayerTurnCount - 1]
                .transform.position.y, transform.position.z) ; 
    }
}
