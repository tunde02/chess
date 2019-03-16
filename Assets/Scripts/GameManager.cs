using System;
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
    public SelectUIManager selectUIManager;
    public GameObject chessBoard;
    public GameObject chessPieceClone;
	
    private GameObject target;
    private ChessPiece selectedChessPiece;
	private readonly Square[,] squares = new Square[8, 8];
    private List<Index> possibleSquares = new List<Index>();
    private bool isChessPieceSelected = false;

    private Player player1;

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

        CreateChessPieces();

        // test case
        player1 = new Player();
    }

    private void CreateChessPieces()
    {
        for(int i=0; i<8; i++)
        {
            for(int j=0; j<2; j++)
            {
                Vector3 temp = squares[j, i].transform.position;
                Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 2, temp.z), Quaternion.identity);
            }
        }
    }

	private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !selectUIManager.IsActive)
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
            Debug.Log(target);
            ResetSelectedChessPiece();

            if (!isChessPieceSelected)
			{
                //selectedChessPiece = target.transform.parent.GetComponent<ChessPiece>();
                selectedChessPiece = target.transform.parent.parent.GetComponent<ChessPiece>();
				isChessPieceSelected = true;

                selectUIManager.SetRemainTexts(player1.GetChessPieceRemains());
                selectUIManager.OpenUI();
			}
		}
		else if (target.tag == "Board Element")
		{
			if (isChessPieceSelected)
			{
				if (target.GetComponent<Square>().Status == "possible")
				{
					Debug.Log("chess piece move!");

					selectedChessPiece.MoveTo(target.transform.position);

					ResetPossibleSquares();
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

    private void SetPossibleSquares()
    {
        Debug.Log("set possible squares");

        // selected chess piece의 type을 읽어와
        // chess piece가 갈 수 있는 칸들을 하이라이트해줌
        switch (selectedChessPiece.GetType())
        {
            case ChessPieceType.Pawn:
                // 수정이 필요하다
                Index index = new Index((int)Math.Round(selectedChessPiece.transform.position.z + 3.5f, MidpointRounding.AwayFromZero) + 1,
                    (int)Math.Round(selectedChessPiece.transform.position.x + 3.5f, MidpointRounding.AwayFromZero));
                //Debug.Log(index.X + ", " + index.Y);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
            default:
                Debug.Log("chess piece type is Normal");
                break;
        }

        // test case
        //Index testIndex = new Index(5, 6);
        //possibleSquares.Add(testIndex);
        //squares[testIndex.X, testIndex.Y].ChangeToSelectedMaterial();
    }

    private void ResetPossibleSquares()
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

    public void SetSelectedChessPiece()
    {
        Debug.Log("Change chess piece type to " + selectUIManager.GetSelectedChessPieceType());
        selectedChessPiece.SetType(selectUIManager.GetSelectedChessPieceType());
        selectUIManager.QuitUI();
        SetPossibleSquares();
    }

    public void ResetSelectedChessPiece()
    {
        if (selectedChessPiece != null)
        {
            selectedChessPiece.SetType(ChessPieceType.Normal);
        }

        isChessPieceSelected = false;

        Debug.Log("reset selected chess piece");
    }
}
