using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUIManager : MonoBehaviour
{
    public Text[] texts;

    private Vector3[] originalPositions = new Vector3[2];

    private void Start()
    {
        originalPositions[0] = texts[0].transform.position;
        originalPositions[1] = texts[1].transform.position;
        ResetText();
    }

    public void ResetText()
    {
        for(int i=0; i<2; i++)
        {
            texts[i].text = "";
            texts[i].transform.position = originalPositions[i];
        }
    }

    public void SetText(int ownerNumber, string text)
    {
        texts[ownerNumber].text = text;
    }
}
