
public class Player
{
    private bool isTurn;
    //private ChessPiece king;
    private int[] chessPieceRemains;
    private bool isKingAlive;

    public Player()
    {
        SetPlayerInfo();
    }

    private void SetPlayerInfo()
    {
        isTurn = false;

        chessPieceRemains = new int[5];
        chessPieceRemains[(int)ChessPieceType.Pawn] = 8;
        chessPieceRemains[(int)ChessPieceType.Rook] = 2;
        chessPieceRemains[(int)ChessPieceType.Knight] = 2;
        chessPieceRemains[(int)ChessPieceType.Bishop] = 2;
        chessPieceRemains[(int)ChessPieceType.Queen] = 1;

        isKingAlive = true;
    }

    public int[] GetChessPieceRemains()
    {
        return chessPieceRemains;
    }
}
