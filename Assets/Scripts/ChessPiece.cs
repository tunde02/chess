using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
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
}
