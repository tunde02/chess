using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType { Pawn, Rook, Knight, Bishop, Queen, Normal }

public class ChessPiece : MonoBehaviour
{
	public GameObject[] forms;// 체스말의 종류를 바꿀 때 사용할 배열
    public float maxHeight = 3.0f;
    public float moveSpeed = 0.09f;

	private ChessPieceType type = ChessPieceType.Normal;

    public void MoveTo(Vector3 destination)
	{
		StartCoroutine(StartMoveTo(destination));
	}

	private IEnumerator StartMoveTo(Vector3 _destination)
	{
        Vector3 destination = transform.position;
        destination.y += maxHeight;

        GetComponent<Rigidbody>().isKinematic = true;

        // 다른 ChessPiece와의 충돌을 피하기 위해 먼저 위로 올림
        while (transform.position.y < maxHeight)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed);
            yield return null;
        }

        destination.x = _destination.x;
        destination.z = _destination.z;

        // 목적지로 이동
        while (transform.position.x != destination.x || transform.position.z != destination.z)
		{
			transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed);
            yield return null;
		}

        // 물리효과에 의해 자연스럽게 낙하
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public new ChessPieceType GetType()
    {
        return type;
    }

    public void SetType(ChessPieceType newType)
	{
        // type에 맞는 체스말로 변경
        Debug.Log((int)type);
        Debug.Log(forms[(int)type]);
        forms[(int)type].SetActive(false);
        Debug.Log(forms[(int)newType]);
        forms[(int)newType].SetActive(true);
        type = newType;
        Debug.Log("New type : " + newType);
    }
}
