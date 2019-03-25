﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public TextUIManager textUIManager;
    public GameObject chessBoard;
    public GameObject chessPieceClone;
    public bool isTest;

    private GameObject target;
    private ChessPiece selectedChessPiece;
	private readonly Square[,] squares = new Square[8, 8];
    private List<Index> possibleSquares = new List<Index>();
    private bool isConfirmed = false;
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

        player[0].StartTurn();
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

			if (isConfirmed && player[turn].currentChessPiece.state == ChessPiece.State.Move)
			{
				return;
			}

			if (targetChessPiece.GetOwner() == player[turn])
            {
				selectedChessPiece = targetChessPiece;

                if (player[turn].currentChessPiece != null)
                {
                    selectUIManager.OpenUI(player[turn].GetChessPieceRemains(), player[turn].currentChessPiece.Type);
                }
                else
                {
                    selectUIManager.OpenUI(player[turn].GetChessPieceRemains(), ChessPieceType.Normal);
                }
			}
			else
            {
				Debug.Log("You Clicked Wrong Chess Piece");
			}
		}
		else if (target.tag == "Board Element")
		{
			if (isConfirmed)
			{
				if (target.GetComponent<Square>().status == "possible")
				{
					Debug.Log("Chess piece move!");


					player[turn].currentChessPiece.MoveTo(target.transform.position);
					StartCoroutine(WaitTurn());

					ResetPossibleSquares();
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
		int waitTurn = turn;

        while (player[turn].currentChessPiece.state != ChessPiece.State.Stop)
        {
            yield return new WaitForSeconds(0.05f);
        }

		isConfirmed = false;

		player[turn].EndTurn();
		turn = turn == 0 ? 1 : 0;
		player[turn].StartTurn();
	}

    public void OnChessPieceTypeButtonClicked()
    {
        selectedChessPiece.ChangeTypeTo(selectUIManager.GetSelectedChessPieceType());
        UpdatePossibleSquares(selectedChessPiece);
    }

    private void UpdatePossibleSquares(ChessPiece chessPiece)
    {
        // TODO: Chess Piece Type에 따른 possible squares 구현 (index를 이용해서)

        Debug.Log("Update Possible Squares About " + chessPiece.Type);
        Debug.Log(MeasureIndex(chessPiece).X + ", " + MeasureIndex(chessPiece).Y);
        ResetPossibleSquares();

        Index index;

        if (isTest)
        {
            switch (chessPiece.Type)
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
                default:
                    break;
            }
            return;
        }

        Index chessPieceIndex = MeasureIndex(chessPiece);
        List<Index> list = new List<Index>();
        //Index index;

        switch (chessPiece.Type)
        {
            case ChessPieceType.Pawn:
                // 1. 첫 전진만 2칸 전진 가능 -> pawn count를 둬서 pawn 개수만큼 pawn 선택시 2칸전진 가능하도록
                // 1.a. pawn count를 어떻게 관리할까?
                // 2. 평소 1칸 전진 가능 (단, 다른 말을 뛰어넘을 수 없다)
                // 3. 상대 말을 잡으려면 1칸 대각선이동해서 잡아야함
                // 4. 앙파상...?
                // 5. 프로모션 -> 폰을 제외한 다른 말들 중 하나를 랜덤하게 +1 하도록

                if ((turn == 0 && chessPieceIndex.X <= 6) || (turn == 1 && chessPieceIndex.X >= 1))
                {
                    // 일반 이동
                    index = new Index(turn == 0 ? chessPieceIndex.X + 1 : chessPieceIndex.X - 1, chessPieceIndex.Y);
                    if (!IsChessPieceThere(index))
                    {
                        list.Add(index);

                        // 2칸 이동
                        if (player[turn].pawnCount > 0 && ((turn == 0 && chessPieceIndex.X <= 5) || (turn == 1 && chessPieceIndex.X >= 2)))
                        {
                            index = new Index(turn == 0 ? chessPieceIndex.X + 2 : chessPieceIndex.X - 2, chessPieceIndex.Y);
                            if (!IsChessPieceThere(index))
                            {
                                list.Add(index);
                            }
                        }
                    }

                    // 상대 말을 잡을 수 있는 경우
                    Index leftCase = new Index(turn == 0 ? chessPieceIndex.X + 1 : chessPieceIndex.X - 1, chessPieceIndex.Y - 1); ;
                    Index rightCase = new Index(turn == 0 ? chessPieceIndex.X + 1 : chessPieceIndex.X - 1, chessPieceIndex.Y + 1); ;

                    if (IsChessPieceThere(leftCase)
                            && !squares[leftCase.X, leftCase.Y].aboveChessPiece.GetOwner().isTurn)
                    {
                        list.Add(leftCase);
                    }
                    if (IsChessPieceThere(rightCase)
                        && !squares[rightCase.X, rightCase.Y].aboveChessPiece.GetOwner().isTurn)
                    {
                        list.Add(rightCase);
                    }
                }
                break;
			case ChessPieceType.Rook:
				bool up = true, down = true, left = true, right = true;
				int i = 1;

				while(up)
				{
                    try
                    {
                        if (squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece != null)
                        {
                            if (squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece.GetOwner() != player[turn])
                            {
                                list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                            }
                            i = 1;
                            up = false;
                            break;
                        }

                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        i = 1;
                        break;
                    }

					if(++i >= 8)
					{
						i = 1;
						up = false;
					}
				}

				while (down)
				{
                    try
                    {
                        if (squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece != null)
                        {
                            if (squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece.GetOwner() != player[turn])
                            {
                                list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                            }
                            i = 1;
                            down = false;
                            break;
                        }

                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        i = 1;
                        break;
                    }

					if (++i >= 8)
					{
						i = 1;
						down = false;
					}
				}

				while (left)
				{
                    try
                    {
                        if (squares[chessPieceIndex.X, chessPieceIndex.Y - i].aboveChessPiece != null)
                        {
                            if (squares[chessPieceIndex.X, chessPieceIndex.Y - i].aboveChessPiece.GetOwner() != player[turn])
                            {
                                list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - i));
                            }
                            i = 1;
                            left = false;
                            break;
                        }

                        list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - i));
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        i = 1;
                        break;
                    }

					if (++i >= 8)
					{
						i = 1;
						left = false;
					}
				}

				while (right)
				{
                    try
                    {
                        if (squares[chessPieceIndex.X, chessPieceIndex.Y + i].aboveChessPiece != null)
                        {
                            if (squares[chessPieceIndex.X, chessPieceIndex.Y + i].aboveChessPiece.GetOwner() != player[turn])
                            {
                                list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + i));
                            }
                            i = 1;
                            right = false;
                            break;
                        }

                        list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + i));
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        i = 1;
                        break;
                    }

					if (++i >= 8)
					{
						i = 1;
						right = false;
					}
				}
				break;
            default:
                Debug.Log("chess piece type is Normal");
                break;
        }

        foreach(Index i in list)
        {
            SetPossibleSquare(i);
        }
    }

    private Index MeasureIndex(ChessPiece chessPiece)
    {
        Vector2 chessPiecePos = new Vector2(chessPiece.transform.position.x, chessPiece.transform.position.z);
        float offset = 0.1f;

        for(int i=0; i<8; i++)
        {
            for(int j=0; j<8; j++)
            {
                Vector2 squarePos = new Vector2(squares[j, i].transform.position.x, squares[j, i].transform.position.z);

                if(Vector2.Distance(chessPiecePos, squarePos) <= offset)
                {
                    return new Index(j, i);
                }
            }
        }

        return null;
    }

    private bool IsChessPieceThere(Index index)
    {
        if (squares[index.X, index.Y].aboveChessPiece == null)
        {
            return false;
        }

        return true;
    }

    private void SetPossibleSquare(Index index)
    {
        possibleSquares.Add(index);
        squares[index.X, index.Y].ChangeToSelectedMaterial();
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
		if (player[turn].currentChessPiece != null && player[turn].currentChessPiece != selectedChessPiece)
		{
			player[turn].currentChessPiece.ChangeTypeTo(ChessPieceType.Normal);
		}

		player[turn].currentChessPiece = selectedChessPiece;
        selectedChessPiece = null;
        isConfirmed = true;

        selectUIManager.QuitUI();
    }

    public void CancelSelectedChessPiece()
    {
        // 타입이 정해져있던 Chess Piece를 클릭해서 타입을 바꾸다가,
        // 취소하면 원래 타입으로 되돌아가야한다.
        selectedChessPiece.ChangeTypeTo(ChessPieceType.Normal);

        if (isConfirmed)
        {
            player[turn].currentChessPiece.ChangeTypeTo(selectUIManager.GetPrevChessPieceType());
            UpdatePossibleSquares(player[turn].currentChessPiece);
        }
        else
        {
            UpdatePossibleSquares(selectedChessPiece);
        }

        selectUIManager.QuitUI();
    }

    public void ShowText(ChessPiece chessPiece)
    {
        string text = "Player " + chessPiece.GetOwner().Number + "'s\n" + chessPiece.Type + " -1";
        textUIManager.SetText(chessPiece.GetOwner().Number, text);
        StartCoroutine(MoveText(textUIManager.texts[chessPiece.GetOwner().Number]));
    }

    private IEnumerator MoveText(Text text)
    {
        Vector3 destination = text.GetComponent<RectTransform>().localPosition;
        destination.y += 100f;

        while(Mathf.Abs(text.GetComponent<RectTransform>().localPosition.y - destination.y) >= 1)
        {
            text.GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(text.GetComponent<RectTransform>().localPosition, destination, 1.0f);
            yield return null;
        }

        textUIManager.ResetText();
    }
}
