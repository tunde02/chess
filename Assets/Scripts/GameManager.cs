using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject square;

    private GameObject target;
    private GameObject selectedChessPiece;
    private List<GameObject> selectedSquares = new List<GameObject>();
    private bool isChessPieceSelected = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = GetClickedObject();

            if(target == null)
            {
                return;
            }

            if (target.tag == "Chess Piece Element")
            {
                if (!isChessPieceSelected)
                {
                    Debug.Log("selected!");

                    selectedChessPiece = target.transform.parent.gameObject;
                    isChessPieceSelected = true;

                    selectedSquares.Add(Instantiate(square, new Vector3(2.5f, 0, -3.5f), Quaternion.identity));
                }
                else
                {
                    selectedChessPiece = null;
                    isChessPieceSelected = false;

                    RemoveSelectedSquares();
                }
            }
            else if(target.tag == "Board Element")
            {
                if (isChessPieceSelected)
                {
                    Debug.Log("chess piece move!");
                    Vector3 destination = new Vector3(target.transform.position.x, 2.0f, target.transform.position.z);

                    StartCoroutine(MoveChessPieceTo(destination));
                }

                RemoveSelectedSquares();
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

    private void RemoveSelectedSquares()
    {
        int size = selectedSquares.Count;
        for(int i=0; i<size; i++)
        {
            Destroy(selectedSquares[i]);
            selectedSquares.Remove(selectedSquares[i]);
        }
    }

    private IEnumerator MoveChessPieceTo(Vector3 destination)
    {
        selectedChessPiece.GetComponent<Rigidbody>().isKinematic = true;

        while (selectedChessPiece.transform.position.x != destination.x || selectedChessPiece.transform.position.z != destination.z)
        {
            selectedChessPiece.transform.position = Vector3.MoveTowards(selectedChessPiece.transform.position, destination, 0.09f);
            yield return new WaitForSeconds(0f);
        }

        selectedChessPiece.GetComponent<Rigidbody>().isKinematic = false;
    }
}
