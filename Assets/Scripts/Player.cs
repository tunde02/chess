using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Number { get; set; }
    private bool isTurn;
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

    public void SetTurn(bool turn)
    {
        isTurn = turn;
    }

    public int[] GetChessPieceRemains()
    {
        return chessPieceRemains;
    }

    public void AddChessPiece(ChessPiece chessPiece)
    {
        //Debug.Log("Add " + chessPiece + " to Player's List");
        chessPieces.Add(chessPiece);
        chessPiece.SetOwner(this);
    }
}
