using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Number { get; set; }
	public ChessPiece currentChessPiece;
	public bool isTurn;
    public int pawnCount;
    public bool isKingAlive;
    public ChessPiece king;
    private List<ChessPiece> chessPieces;
    private int[] chessPieceRemains;

    public Player(int number)
    {
        Number = number;
        SetPlayerInfo();
    }

    private void SetPlayerInfo()
    {
        isTurn = false;

        chessPieces = new List<ChessPiece>();

        chessPieceRemains = new int[6];
        chessPieceRemains[(int)ChessPieceType.Pawn] = 8;
        chessPieceRemains[(int)ChessPieceType.Rook] = 2;
        chessPieceRemains[(int)ChessPieceType.Knight] = 2;
        chessPieceRemains[(int)ChessPieceType.Bishop] = 2;
        chessPieceRemains[(int)ChessPieceType.Queen] = 1;

        isKingAlive = true;

        pawnCount = 8;
    }

    public void ResetPlayerInfo()
    {
        foreach (ChessPiece chessPiece in chessPieces)
        {
            chessPiece.PerformDestroyEvent();
        }

        if (isKingAlive)
        {
            king.PerformDestroyEvent();
        }

        chessPieces.Clear();
        SetPlayerInfo();
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

    public void AddKing(ChessPiece chessPieceKing)
    {
        king = chessPieceKing;
        chessPieceKing.SetOwner(this);
        chessPieceKing.ChangeTypeTo(ChessPieceType.King);
    }

    public void MinusChessPiece(ChessPiece chessPiece)
	{
        if (chessPiece.Type == ChessPieceType.King)
        {
            Debug.Log(Number + "'s king dead");
            isKingAlive = false;
        }
        else if(chessPiece.Type == ChessPieceType.Normal)
        {
            int randomNumber = Random.Range(0, 4);
            while (chessPieceRemains[randomNumber] <= 0)
            {
                randomNumber = Random.Range(0, 4);
            }
            chessPieceRemains[randomNumber]--;
            chessPieces.Remove(chessPiece);
        }
        else
        {
            chessPieceRemains[(int)chessPiece.Type]--;
            chessPieces.Remove(chessPiece);
        }
	}
}
