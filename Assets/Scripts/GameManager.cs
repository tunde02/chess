using System;
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
    public SelectUIManager selectUIManager;
    public GameObject chessBoard;
    public GameObject chessPieceClone;
	
    private GameObject target;
    private ChessPiece selectedChessPiece;
    private ChessPiece currentlySelectedChessPiece;
	private readonly Square[,] squares = new Square[8, 8];
    private List<Index> possibleSquares = new List<Index>();
    private bool isConfirmed = false;// 굳이 필요할까? null검사하면 되지 않나

    private int turn = 0;
    private Player[] player = new Player[2];

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

        // test case
        player[0] = new Player(0);
        player[1] = new Player(1);

        CreateChessPieces();

        player[0].SetTurn(true);

        Debug.Log("Player" + turn + "'s turn");
    }

    private void CreateChessPieces()
    {
        for(int i=0; i<8; i++)
        {
            for(int j=0; j<2; j++)
            {
                Vector3 temp = squares[j, i].transform.position;
                player[0].AddChessPiece(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
            }

            for(int j=6; j<8; j++)
            {
                Vector3 temp = squares[j, i].transform.position;
                player[1].AddChessPiece(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
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
            Debug.Log("You Clicked " + target);
            
            ChessPiece targetChessPiece = target.transform.parent.GetComponent<ChessPiece>();

            if (targetChessPiece.GetOwner() != player[turn])
            {
                Debug.Log("You Clicked Wrong Chess Piece!!");
            }
            else
            {
                currentlySelectedChessPiece = targetChessPiece;

                selectUIManager.SetRemainTexts(player[turn].GetChessPieceRemains());
                selectUIManager.OpenUI();
            }
		}
		else if (target.tag == "Board Element")
		{
			if (isConfirmed)
			{
				if (target.GetComponent<Square>().Status == "possible")
				{
					Debug.Log("Chess piece move!");

					selectedChessPiece.MoveTo(target.transform.position);

					ResetPossibleSquares();
                    //StartCoroutine(WaitTurn());
                    //ResetSelectedChessPiece();

                    player[turn].SetTurn(false);
                    turn = turn == 0 ? 1 : 0;
                    player[turn].SetTurn(true);
                    Debug.Log("Player" + turn + "'s turn");
                }
				else
				{
					Debug.Log("Chess piece can't go to there!");
				}
			}
            else
            {
                Debug.Log("Select Chess Piece, First");
            }
		}
		else
		{
			Debug.Log("something selected!");
		}
	}

    private IEnumerator WaitTurn()
    {
        Debug.Log("start wait turn()");
        int currentTurn = turn;
        yield return new WaitForSeconds(0.5f);

        while (currentTurn != turn)
        {
            yield return new WaitForSeconds(0.05f);
        }

        ResetSelectedChessPiece();
    }

    public void OnChessPieceTypeButtonClicked()
    {
        Debug.Log("==OnChessPieceTypeButtonClicked==");
        currentlySelectedChessPiece.ChangeTypeTo(selectUIManager.GetSelectedChessPieceType());
        UpdatePossibleSquares(currentlySelectedChessPiece);
        Debug.Log("=================================");
    }

    private void UpdatePossibleSquares(ChessPiece chessPiece)
    {
        // TODO: Chess Piece Type에 따른 possible squares 구현 (index를 이용해서)

        Debug.Log("Update Possible Squares About " + chessPiece.Type);

        ResetPossibleSquares();

        // TEST CASE
        Index index;
        switch(chessPiece.Type)
        {
            case ChessPieceType.Pawn:
                index = new Index(3, 1);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
            case ChessPieceType.Rook:
                index = new Index(3, 2);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
            case ChessPieceType.Knight:
                index = new Index(3, 3);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
            case ChessPieceType.Bishop:
                index = new Index(3, 4);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
            case ChessPieceType.Queen:
                index = new Index(3, 5);
                possibleSquares.Add(index);
                squares[index.X, index.Y].ChangeToSelectedMaterial();
                break;
        }
        

        // selected chess piece의 type을 읽어와
        // chess piece가 갈 수 있는 칸들을 하이라이트해줌

        //switch (selectedChessPiece.GetType())
        //{
        //    case ChessPieceType.Pawn:
        //        // 수정이 필요하다
        //        Index index = new Index((int)Math.Round(selectedChessPiece.transform.position.z + 3.5f, MidpointRounding.AwayFromZero) + 1,
        //            (int)Math.Round(selectedChessPiece.transform.position.x + 3.5f, MidpointRounding.AwayFromZero));
        //        //Debug.Log(index.X + ", " + index.Y);
        //        possibleSquares.Add(index);
        //        squares[index.X, index.Y].ChangeToSelectedMaterial();
        //        break;
        //    default:
        //        Debug.Log("chess piece type is Normal");
        //        break;
        //}
    }

    private void ResetPossibleSquares()
    {
        int size = possibleSquares.Count;

		// red컬러로 변경된 Square들을 원상태로 되돌림
		for (int i=0; i<size; i++)
        {
			squares[possibleSquares[i].X, possibleSquares[i].Y].ChangeToOriginalMaterial();
        }

		possibleSquares.Clear();
    }

    public void ConfirmSelectedChessPiece()
    {
        ResetSelectedChessPiece();

        selectedChessPiece = currentlySelectedChessPiece;
        //currentlySelectedChessPiece = null; 필요할까?
        isConfirmed = true;

        selectUIManager.QuitUI();
    }

    public void CancelSelectedChessPiece()
    {
        Debug.Log("Cancel Selecting Chess Piece Type");

        if (currentlySelectedChessPiece != null)
        {
            currentlySelectedChessPiece.ChangeTypeTo(ChessPieceType.Normal);
        }

        if (selectedChessPiece != null)
        {
            UpdatePossibleSquares(selectedChessPiece);
        }

        selectUIManager.QuitUI();
    }

    private void ResetSelectedChessPiece()
    {
        Debug.Log("Reset selected chess piece");

        if (selectedChessPiece != null)
        {
            selectedChessPiece.ChangeTypeTo(ChessPieceType.Normal);
        }

        isConfirmed = false;
    }
}
