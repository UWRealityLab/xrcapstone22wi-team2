using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBall : MonoBehaviour
{
    
    [SerializeField]
    public Material hoverMaterial;

    private Material initialMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        initialMaterial = GetComponent<Renderer>().material;
    }

    public void startHoverColor()
    {
        GetComponent<Renderer>().material = hoverMaterial;
    }

    public void endHoverColor()
    {
        GetComponent<Renderer>().material = initialMaterial;
    }

}
