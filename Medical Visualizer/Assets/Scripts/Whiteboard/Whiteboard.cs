using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    public CrossSectionSphere sphere1;
    public CrossSectionSphere sphere2;
    public CrossSectionSphere sphere3;

    void Start()
    {
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;
    }

    private void Update()
    {
        if (sphere1 != null && sphere2 != null && sphere3 != null)
        {
            Vector3 ab = sphere1.transform.position - sphere2.transform.position;
            Vector3 bc = sphere2.transform.position - sphere3.transform.position;

            Vector3 center = (sphere1.transform.position + sphere2.transform.position + sphere3.transform.position) / 3;
            Vector3 direction = Vector3.Cross(ab, bc);
            Quaternion rotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(270, 0, 0);
            transform.rotation = rotation;
            transform.position = center;
        }
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
