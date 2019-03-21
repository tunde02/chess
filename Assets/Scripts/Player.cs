using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Number { get; set; }
	public ChessPiece currentChessPiece;
	public bool isTurn;
    private ChessPiece king;
    private List<ChessPiece> chessPieces;
    private int[] chessPieceRemains;
    private bool isKingAlive;

    public Player(int number)
    {
        Number = number;
        SetPlayerInfo();
    }

    private void SetPlayerInfo()
    {
        isTurn = false;

        chessPieces = new List<ChessPiece>();

        chessPieceRemains = new int[5];
        chessPieceRemains[(int)ChessPieceType.Pawn] = 8;
        chessPieceRemains[(int)ChessPieceType.Rook] = 2;
        chessPieceRemains[(int)ChessPieceType.Knight] = 2;
        chessPieceRemains[(int)ChessPieceType.Bishop] = 2;
        chessPieceRemains[(int)ChessPieceType.Queen] = 1;

        isKingAlive = true;
    }

	public void StartTurn()
	{
		Debug.Log("Player " + Number + "'s turn Start");
		isTurn = true;

		if (currentChessPiece != null)
		{
			currentChessPiece.ChangeTypeTo(ChessPieceType.Normal);
			currentChessPiece = null;
		}
	}

	public void EndTurn()
	{
		isTurn = false;
	}

    public int[] GetChessPieceRemains()
    {
        return chessPieceRemains;
    }

    public void AddChessPiece(ChessPiece chessPiece)
    {
        chessPieces.Add(chessPiece);
        chessPiece.SetOwner(this);
    }

	public void MinusChessPiece(ChessPiece chessPiece)
	{
		// King을 Chess Piece Type에 넣어서 if문으로 구분할지 아니면 따로 만들지

		chessPieceRemains[(int)chessPiece.Type]--;
		chessPieces.Remove(chessPiece);// 잘 찾을 수 있을까
	}
}
