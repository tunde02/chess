using System.Collections;
using UnityEngine;

public enum ChessPieceType { Pawn, Rook, Knight, Bishop, Queen, Normal }

public class ChessPiece : MonoBehaviour
{
	public enum State
	{
		Stop,
		Move
	}

	public GameObject[] forms;// 체스말의 종류를 바꿀 때 사용할 배열
    public float maxHeight = 3.0f;
    public float moveSpeed = 0.09f;
    public Material whiteColor;// default color = black

	public ChessPieceType Type { get; set; }
	public State state;
    private GameManager gm;
    private Player owner;
    private int destroyCount = 1;

	private void Start()
	{
		for (int i = 0; i < 6; i++)
		{
			forms[i] = transform.GetChild(i).gameObject;
		}
		
		Type = ChessPieceType.Normal;
		state = State.Stop;

        if (owner.Number == 1)
        {
            ChangeColorToWhite();
        }

        gm = FindObjectOfType<GameManager>();
    }

	public void MoveTo(Vector3 destination)
	{
		StartCoroutine(StartMoveTo(destination));
	}

	private IEnumerator StartMoveTo(Vector3 destination)
	{
        Vector3 _destination = transform.position;
        _destination.y += maxHeight;

		state = State.Move;

        GetComponent<Rigidbody>().isKinematic = true;

        // 다른 ChessPiece와의 충돌을 피하기 위해 먼저 위로 올림
        while (transform.position.y < _destination.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, moveSpeed);
            yield return null;
        }

        _destination.x = destination.x;
        _destination.z = destination.z;
        
        // 목적지로 이동
        while (transform.position.x != _destination.x || transform.position.z != _destination.z)
		{
			transform.position = Vector3.MoveTowards(transform.position, _destination, moveSpeed);
            yield return null;
		}

        // 물리효과에 의해 자연스럽게 낙하
        GetComponent<Rigidbody>().isKinematic = false;
	}

    public void ChangeTypeTo(ChessPieceType newType)
	{
        // type에 맞는 체스말로 변경
        forms[(int)Type].SetActive(false);
        forms[(int)newType].SetActive(true);
        Type = newType;

        // 흰 색 플레이어일 경우 색을 바꿔준다
        if (owner.Number == 1)
        {
            ChangeColorToWhite();
        }
    }

    private void ChangeColorToWhite()
    {
        MeshRenderer[] renderers = forms[(int)Type].GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = whiteColor;
        }
    }

    public Player GetOwner()
    {
        return owner;
    }

    public void SetOwner(Player player)
    {
        owner = player;
	}
    
	public void ChangeType()
	{
		forms[(int)Type].SetActive(false);
		Type = (ChessPieceType)(((int)++Type) % 6);
		forms[(int)Type].SetActive(true);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Chess Piece Element" && !owner.isTurn && destroyCount > 0)
        {
            --destroyCount;
            owner.MinusChessPiece(this);
            PerformDestroyEvent();
        }
    }
    
    private void PerformDestroyEvent()
	{
        ShowDestroyText();
		Destroy(gameObject);
	}

    private void ShowDestroyText()
    {
        gm.ShowText(this);
    }
}
