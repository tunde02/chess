using System;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Material selectedMaterial;

    public string status;
    public ChessPiece aboveChessPiece;
    private MeshRenderer render;
    private Material originalMaterial;
	
    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        originalMaterial = render.material;
		status = "original";
    }

    public void ChangeToSelectedMaterial()
    {
        render.material = selectedMaterial;
		status = "possible";
    }

    public void ChangeToOriginalMaterial()
    {
        render.material = originalMaterial;
		status = "original";
    }

	private void OnCollisionEnter(Collision collision)
	{
        aboveChessPiece = collision.gameObject.GetComponent<ChessPiece>();

        aboveChessPiece.state = ChessPiece.State.Stop;
	}

    private void OnCollisionExit(Collision collision)
    {
        aboveChessPiece = null;
    }
}
