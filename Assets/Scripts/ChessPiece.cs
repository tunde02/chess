using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType { Normal, Pawn, Rook, Knight, Bishop, Queen, King }

public class ChessPiece : MonoBehaviour
{
	public GameObject[] forms;// 체스말의 종류를 바꿀 때 사용할 배열

	private ChessPieceType type = ChessPieceType.Normal;
	
	public void MoveTo(Vector3 destination)
	{
		StartCoroutine(StartMoveTo(destination));
	}

	private IEnumerator StartMoveTo(Vector3 destination)
	{
		//GameObject _chessPiece = chessPiece;
		GetComponent<Rigidbody>().isKinematic = true;

		while (transform.position.x != destination.x || transform.position.z != destination.z)
		{
			transform.position = Vector3.MoveTowards(transform.position, destination, 0.09f);
			yield return new WaitForSeconds(0f);
		}

		GetComponent<Rigidbody>().isKinematic = false;
	}

	public void ChangeTypeTo(ChessPieceType newType)
	{
		type = newType;
		// 이 체스말을 type에 맞는 체스말로 변경
	}
}
