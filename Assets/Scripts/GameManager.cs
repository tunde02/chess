using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Index
{
	public int X { get; set; }
	public int Y { get; set; }
	public Index(int x, int y) { X = x; Y = y; }
}

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
	public GameObject chessBoard;

    private GameObject target;
    private ChessPiece selectedChessPiece;
	private Square[,] squares = new Square[8, 8];
    private List<Index> possibleSquares = new List<Index>();
    private bool isChessPieceSelected = false;

	private void Start()
	{
		int index = 0;
		for(int i=0; i<8; i++)
		{
			for(int j=0; j<8; j++)
			{
				squares[i, j] = chessBoard.transform.GetChild(index++).gameObject.GetComponent<Square>();
			}
		}
	}

	private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if((target = GetClickedObject()) != null)
			{
				PerformSelectedEvent();
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

	private void PerformSelectedEvent()
	{
		if (target.tag == "Chess Piece Element")
		{
			if (!isChessPieceSelected)
			{
				Debug.Log("selected!");

				selectedChessPiece = target.transform.parent.GetComponent<ChessPiece>();
				isChessPieceSelected = true;

				// 그 Chess Piece가 이동할 수 있는 Square를 표시
				squares[3, 3].ChangeToSelectedMaterial();
				possibleSquares.Add(new Index(3, 3));
			}
			else
			{
				Debug.Log("already selected");

				RemoveSelectedSquares();
			}
		}
		else if (target.tag == "Board Element")
		{
			if (isChessPieceSelected)
			{
				if (target.GetComponent<Square>().Status == "selected")
				{
					Debug.Log("chess piece move!");
					Vector3 destination = new Vector3(target.transform.position.x, 2.0f, target.transform.position.z);

					selectedChessPiece.MoveTo(destination);

					RemoveSelectedSquares();
				}
				else
				{
					Debug.Log("chess piece can't go to there!");
				}
			}
		}
		else
		{
			Debug.Log("something selected!");
		}
	}

    private void RemoveSelectedSquares()
    {
        int size = possibleSquares.Count;

		// 선택되어있던 Chess Piece와 selected플래그를 초기화
        selectedChessPiece = null;
        isChessPieceSelected = false;

		// red컬러로 변경된 Square들을 원상태로 되돌림
		for (int i=0; i<size; i++)
        {
			squares[possibleSquares[i].X, possibleSquares[i].Y].ChangeToOriginalMaterial();
        }

		possibleSquares.Clear();
    }
}
