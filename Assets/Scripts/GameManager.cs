using System.Collections;
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
    public MenuUIManager menuUIManager;
    public GameObject chessBoard;
    public GameObject chessPieceClone;

    private GameObject target;
    private ChessPiece selectedChessPiece;
	private readonly Square[,] squares = new Square[8, 8];
    private List<Index> possibleSquares = new List<Index>();
    private bool isConfirmed = false;
    private int turn = 0;
    private Player[] player = new Player[2];
    private bool isOver = false;

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

    public void RestartGame()
    {
        player[0].ResetPlayerInfo();
        player[1].ResetPlayerInfo();

        CreateChessPieces();

        menuUIManager.gameObject.SetActive(false);

        turn = 0;
        player[0].StartTurn();
        player[1].EndTurn();

        isOver = false;
    }

    private void CreateChessPieces()
    {
        for(int i=0; i<8; i++)
        {
            for(int j=0; j<2; j++)
            {
                Vector3 temp = squares[j, i].transform.position;

                if (i == 4 && j == 0)
                {
                    player[0].AddKing(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
                    Debug.Log(player[0].king.GetOwner().Number);
                    continue;
                }

                player[0].AddChessPiece(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
            }

            for(int j=6; j<8; j++)
            {
                Vector3 temp = squares[j, i].transform.position;

                if (i == 4 && j == 7)
                {
                    player[1].AddKing(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
                    Debug.Log(player[1].king.GetOwner().Number);
                    continue;
                }

                player[1].AddChessPiece(Instantiate(chessPieceClone, new Vector3(temp.x, temp.y + 1, temp.z), Quaternion.identity).GetComponent<ChessPiece>());
            }
        }
    }

	private void Update()
    {
        if (!isOver && Input.GetMouseButtonDown(0) && !selectUIManager.IsActive)
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
        if (target.tag == "Board Element")
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
        else if (target.tag == "Chess Piece Element")
        {
            ChessPiece targetChessPiece = target.transform.parent.GetComponent<ChessPiece>();

            if(targetChessPiece.Status == "possible")
            {
                Square tempSquare = null;
                for(int i=0; i<possibleSquares.Count; i++)
                {
                    if(squares[possibleSquares[i].X, possibleSquares[i].Y].aboveChessPiece == targetChessPiece)
                    {
                        tempSquare = squares[possibleSquares[i].X, possibleSquares[i].Y];
                    }
                }

                player[turn].currentChessPiece.MoveTo(tempSquare.transform.position);
                StartCoroutine(WaitTurn());

                ResetPossibleSquares();
            }
            else
            {
                // Chess Piece가 움직이는 중이라면 무시
                if (isConfirmed && player[turn].currentChessPiece.state == ChessPiece.State.Move)
                {
                    return;
                }

                if (targetChessPiece.GetOwner() == player[turn])
                {
                    selectedChessPiece = targetChessPiece;

                    if (selectedChessPiece.Type == ChessPieceType.King)
                    {
                        player[turn].currentChessPiece = selectedChessPiece;
                        selectedChessPiece = null;
                        isConfirmed = true;
                        UpdatePossibleSquares(player[turn].currentChessPiece);
                    }
                    else
                    {
                        if (player[turn].currentChessPiece != null)
                        {
                            selectUIManager.OpenUI(player[turn].GetChessPieceRemains(), player[turn].currentChessPiece.Type);
                        }
                        else
                        {
                            selectUIManager.OpenUI(player[turn].GetChessPieceRemains(), ChessPieceType.Normal);
                        }
                    }
                }
                else
                {
                    Debug.Log("You Clicked Wrong Chess Piece");
                }
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
        if (player[turn].GetChessPieceRemains()[(int)selectUIManager.GetSelectedChessPieceType()] > 0)
        {
            selectedChessPiece.ChangeTypeTo(selectUIManager.GetSelectedChessPieceType());
            UpdatePossibleSquares(selectedChessPiece);
        }
    }

    private void UpdatePossibleSquares(ChessPiece chessPiece)
    {
        ResetPossibleSquares();

        Index chessPieceIndex = MeasureIndex(chessPiece);
        List<Index> list = new List<Index>();
        Index index;

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

					try
					{
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
					} catch(System.IndexOutOfRangeException) { }
                }
                break;
			case ChessPieceType.Rook:
                bool[] checks_Rook = new bool[4];

                for (int i = 1; i < 8; i++)
                {
                    if (!checks_Rook[0])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece != null)
                            {
                                checks_Rook[0] = true;

                                if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if (!checks_Rook[1])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece != null)
                            {
                                checks_Rook[1] = true;

                                if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if (!checks_Rook[2])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X, chessPieceIndex.Y - i].aboveChessPiece != null)
                            {
                                checks_Rook[2] = true;

                                if (!squares[chessPieceIndex.X, chessPieceIndex.Y - i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if (!checks_Rook[3])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X, chessPieceIndex.Y + i].aboveChessPiece != null)
                            {
                                checks_Rook[3] = true;

                                if (!squares[chessPieceIndex.X, chessPieceIndex.Y + i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }
                }
				break;
            case ChessPieceType.Knight:
                for (int i = 1; i < 3; i++)
                {
                    int j = 3 - i;

                    try
                    {
                        if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y + j].aboveChessPiece.GetOwner().isTurn)
                        {
                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                        }
                    }
                    catch (System.IndexOutOfRangeException) { }
                    catch (System.NullReferenceException)
                    {
                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                    }

                    try
                    {
                        if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y - j].aboveChessPiece.GetOwner().isTurn)
                        {
                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - j));
                        }
                    }
                    catch (System.IndexOutOfRangeException) { }
                    catch (System.NullReferenceException)
                    {
                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - j));
                    }

                    try
                    {
                        if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y + j].aboveChessPiece.GetOwner().isTurn)
                        {
                            list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + j));
                        }
                    }
                    catch (System.IndexOutOfRangeException) { }
                    catch (System.NullReferenceException)
                    {
                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + j));
                    }

                    try
                    {
                        if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y - j].aboveChessPiece.GetOwner().isTurn)
                        {
                            list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - j));
                        }
                    }
                    catch (System.IndexOutOfRangeException) { }
                    catch (System.NullReferenceException)
                    {
                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - j));
                    }
                }
                
                break;
            case ChessPieceType.Bishop:
                bool[] checks_Bishop = new bool[4];

                for(int i=1; i<8; i++)
                {
                    if (!checks_Bishop[0])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X + i, chessPieceIndex.Y + i].aboveChessPiece != null)
                            {
                                checks_Bishop[0] = true;

                                if(!squares[chessPieceIndex.X + i, chessPieceIndex.Y + i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if(!checks_Bishop[1])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X + i, chessPieceIndex.Y - i].aboveChessPiece != null)
                            {
                                checks_Bishop[1] = true;

                                if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y - i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if (!checks_Bishop[2])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X - i, chessPieceIndex.Y + i].aboveChessPiece != null)
                            {
                                checks_Bishop[2] = true;

                                if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y + i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                    if (!checks_Bishop[3])
                    {
                        try
                        {
                            if (squares[chessPieceIndex.X - i, chessPieceIndex.Y - i].aboveChessPiece != null)
                            {
                                checks_Bishop[3] = true;

                                if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y - i].aboveChessPiece.GetOwner().isTurn)
                                {
                                    list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - i));
                                }
                            }
                            else
                            {
                                list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - i));
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }
                }
                break;
            case ChessPieceType.Queen:
                bool[] checks_Queen = new bool[8];

                for(int i=0; i<8; i++)
                {
                    for(int j=0; j<8; j++)
                    {
                        if(i == 0 && j == 0) { continue; }

                        if (j == 0)// UP, DOWN
                        {
                            if (!checks_Queen[0])
                            {
                                try
                                {
                                    if(squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece != null)
                                    {
                                        checks_Queen[0] = true;

                                        if(!squares[chessPieceIndex.X + i, chessPieceIndex.Y].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }

                            if (!checks_Queen[4])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece != null)
                                    {
                                        checks_Queen[4] = true;

                                        if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }
                        }

                        if(i == 0)// LEFT, RIGHT
                        {
                            if (!checks_Queen[6])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X, chessPieceIndex.Y - j].aboveChessPiece != null)
                                    {
                                        checks_Queen[6] = true;

                                        if (!squares[chessPieceIndex.X, chessPieceIndex.Y - j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y - j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }

                            if (!checks_Queen[2])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X, chessPieceIndex.Y + j].aboveChessPiece != null)
                                    {
                                        checks_Queen[2] = true;

                                        if (!squares[chessPieceIndex.X, chessPieceIndex.Y + j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X, chessPieceIndex.Y + j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }
                        }

                        if(i == j)// y = x 그래프, y = -x 그래프
                        {
                            if (!checks_Queen[1])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X + i, chessPieceIndex.Y + j].aboveChessPiece != null)
                                    {
                                        checks_Queen[1] = true;

                                        if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y + j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }

                            if (!checks_Queen[5])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X - i, chessPieceIndex.Y - j].aboveChessPiece != null)
                                    {
                                        checks_Queen[5] = true;

                                        if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y - j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y - j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }

                            if (!checks_Queen[7])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X + i, chessPieceIndex.Y - j].aboveChessPiece != null)
                                    {
                                        checks_Queen[7] = true;

                                        if (!squares[chessPieceIndex.X + i, chessPieceIndex.Y - j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y - j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }

                            if (!checks_Queen[3])
                            {
                                try
                                {
                                    if (squares[chessPieceIndex.X - i, chessPieceIndex.Y + j].aboveChessPiece != null)
                                    {
                                        checks_Queen[3] = true;

                                        if (!squares[chessPieceIndex.X - i, chessPieceIndex.Y + j].aboveChessPiece.GetOwner().isTurn)
                                        {
                                            list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + j));
                                        }
                                    }
                                    else
                                    {
                                        list.Add(new Index(chessPieceIndex.X - i, chessPieceIndex.Y + j));
                                    }
                                }
                                catch (System.IndexOutOfRangeException) { }
                            }
                        }
                    }
                }
                break;
            case ChessPieceType.King:
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }

                        try
                        {
                            if (squares[chessPieceIndex.X + i, chessPieceIndex.Y + j].aboveChessPiece.GetOwner() == player[turn])
                            {
                                continue;
                            }

                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                        }
                        catch (System.IndexOutOfRangeException) { }
                        catch (System.NullReferenceException)
                        {
                            list.Add(new Index(chessPieceIndex.X + i, chessPieceIndex.Y + j));
                        }
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
		try
		{
			if (squares[index.X, index.Y].aboveChessPiece == null)
			{
				return false;
			}
		} catch(System.IndexOutOfRangeException) { }

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
        try
        {
            if (player[turn].currentChessPiece != selectedChessPiece
                && player[turn].currentChessPiece.Type != ChessPieceType.King)
            {
                player[turn].currentChessPiece.ChangeTypeTo(ChessPieceType.Normal);
            }
        }
        catch (System.NullReferenceException) { }
        catch(MissingReferenceException) { }

		player[turn].currentChessPiece = selectedChessPiece;
        selectedChessPiece = null;
        isConfirmed = true;

        selectUIManager.QuitUI();
    }

    public void CancelSelectedChessPiece()
    {
		// 타입이 정해져있던 Chess Piece를 클릭해서 타입을 바꾸다가,
		// 취소하면 원래 타입으로 되돌아가야한다.

		//if (selectedChessPiece.Type != ChessPieceType.King)
		//{
		//    selectedChessPiece.ChangeTypeTo(ChessPieceType.Normal);
		//}

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

    public void PerformGameOver()
    {
        Debug.Log("game over");
        isOver = true;
        menuUIManager.gameObject.SetActive(true);
    }
}
