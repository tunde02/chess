using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Material selectedMaterial;

    private MeshRenderer render;
    private Material originalMaterial;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        originalMaterial = render.material;
    }

    public void ChangeToSelectedMaterial()
    {
        render.material = selectedMaterial;
    }

    public void ChangeToOriginalMaterial()
    {
        render.material = originalMaterial;
    }
}
