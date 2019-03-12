using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    private Vector3 destination = new Vector3(-3.5f, 2.0f, -3.5f);

    private void Start()
    {
        StartCoroutine(MoveChessPiece());
    }

    private IEnumerator MoveChessPiece()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        while (transform.position.x != destination.x || transform.position.z != destination.z)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 0.09f);
            yield return new WaitForSeconds(0f);
        }

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
