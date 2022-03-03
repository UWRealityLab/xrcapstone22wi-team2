using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeBall : MonoBehaviour
{
    
    [SerializeField]
    public Material hoverMaterial;

    [SerializeField]
    public GameObject sphere;

    [SerializeField]
    public Transform transform;

    private Material initialMaterial;

    private int x;
    
    // Start is called before the first frame update
    void Start()
    {
        x = 1;
        initialMaterial = sphere.GetComponent<Renderer>().material;
    }

    public void changeColor()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("rpcChangeColor", RpcTarget.All, x);
    }

    public void spawnBall()
    {
        GameObject parent = PhotonNetwork.Instantiate("ballTest", transform.position, transform.rotation);
        GameObject obj = PhotonNetwork.Instantiate("Sphere", transform.position, transform.rotation);
        //parent.AddComponent<Sphere>(obj);
        obj.transform.parent = parent.transform;
        //obj.GetComponent<Renderer>().material = hoverMaterial;
    }
    void rpcChangeColor(int other_x)
    {
        Debug.Log("Change color is called");
        if (x % 2 == 1)
        {
            sphere.GetComponent<Renderer>().material = hoverMaterial;
        }
        else
        {
            sphere.GetComponent<Renderer>().material = initialMaterial;
        }
        x++;
    }
}
