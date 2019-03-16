using System;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Material selectedMaterial;

    private MeshRenderer render;
    private Material originalMaterial;
	public string Status { get; set; }

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        originalMaterial = render.material;
		Status = "original";
    }

    public void ChangeToSelectedMaterial()
    {
        render.material = selectedMaterial;
		Status = "possible";
    }

    public void ChangeToOriginalMaterial()
    {
        render.material = originalMaterial;
		Status = "original";
    }
}
