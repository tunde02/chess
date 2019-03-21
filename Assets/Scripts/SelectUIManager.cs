using UnityEngine;
using UnityEngine.UI;

public class SelectUIManager : MonoBehaviour
{
    public Text[] remainTexts;

    public bool IsActive { get; set; }

    private ChessPieceType selectedChessPieceType;
    private ChessPieceType prevChessPieceType;

    public void OpenUI(int[] remains, ChessPieceType prevType)
    {
        SetRemainTexts(remains);
        IsActive = true;
        selectedChessPieceType = ChessPieceType.Normal;
        prevChessPieceType = prevType;
        gameObject.SetActive(true);
    }

    public void QuitUI()
    {
        IsActive = false;
        selectedChessPieceType = ChessPieceType.Normal;
        gameObject.SetActive(false);
    }

    private void SetRemainTexts(int[] remains)
    {
        remainTexts[(int)ChessPieceType.Pawn].text = remains[(int)ChessPieceType.Pawn].ToString();
        remainTexts[(int)ChessPieceType.Rook].text = remains[(int)ChessPieceType.Rook].ToString();
        remainTexts[(int)ChessPieceType.Knight].text = remains[(int)ChessPieceType.Knight].ToString();
        remainTexts[(int)ChessPieceType.Bishop].text = remains[(int)ChessPieceType.Bishop].ToString();
        remainTexts[(int)ChessPieceType.Queen].text = remains[(int)ChessPieceType.Queen].ToString();
    }

    public ChessPieceType GetSelectedChessPieceType()
    {
        return selectedChessPieceType;
    }

    public ChessPieceType GetPrevChessPieceType()
    {
        return prevChessPieceType;
    }

    public void SetSelectedChessPieceType(string typeName)
    {
        ChessPieceType type;

        switch(typeName)
        {
            case "Pawn":
                type = ChessPieceType.Pawn;
                break;
            case "Rook":
                type = ChessPieceType.Rook;
                break;
            case "Knight":
                type = ChessPieceType.Knight;
                break;
            case "Bishop":
                type = ChessPieceType.Bishop;
                break;
            case "Queen":
                type = ChessPieceType.Queen;
                break;
            default:
                type = ChessPieceType.Normal;
                break;
        }

        selectedChessPieceType = type;
        Debug.Log(typeName + " selected");
    }
}
