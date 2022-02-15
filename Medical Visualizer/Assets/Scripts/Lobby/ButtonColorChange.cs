using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{

    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        originalColor = GetComponent<Image>().color;
    }

    public void onHoverColor()
    {
        GetComponent<Image>().color = Color.blue;
    }

    public void endHoverColor()
    {
        GetComponent<Image>().color = originalColor;
    }
}
