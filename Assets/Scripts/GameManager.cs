using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject board;

    private GameObject target;
    private Square[] squares;

    private void Start()
    {
        squares = board.GetComponentsInChildren<Square>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = GetClickedObject();

            if (target != null && target.tag == "Chess Piece Element")
            {
                // write code here
                Debug.Log("selected!");

                // 작동하는 것 확인
                squares[10].ChangeToSelectedMaterial();
                squares[15].ChangeToSelectedMaterial();
            }
            else
            {
                Debug.Log("doesn't selected!");
                squares[10].ChangeToOriginalMaterial();
                squares[15].ChangeToOriginalMaterial();
            }
        }
    }

    private GameObject GetClickedObject()
    {
        RaycastHit hit;
        GameObject target = null;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);//마우스 포인트 근처 좌표를 만든다. 

        if (true == (Physics.Raycast(ray.origin, ray.direction * 10, out hit)))//마우스 근처에 오브젝트가 있는지 확인
        {
            //있으면 오브젝트를 저장한다.
            target = hit.collider.gameObject;
        }

        return target;
    }
}
