using System;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Material selectedMaterial;
    public Material blackMaterial;
    public Material whiteMaterial;

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
        if(aboveChessPiece != null)
        {
            aboveChessPiece.ChangeColorTo(selectedMaterial);
            aboveChessPiece.Status = "possible";
        }
		status = "possible";
    }

    public void ChangeToOriginalMaterial()
    {
        render.material = originalMaterial;
        if (aboveChessPiece != null)
        {
            aboveChessPiece.ChangeColorTo(aboveChessPiece.GetOwner().Number == 0 ? blackMaterial : whiteMaterial);
            aboveChessPiece.Status = "original";
        }
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
