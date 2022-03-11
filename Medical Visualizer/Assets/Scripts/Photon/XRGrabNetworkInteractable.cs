using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        photonView.RequestOwnership();
        GameObject locomotion = GameObject.Find("Locomotion System");
        ActionBasedSnapTurnProvider snapTurn = locomotion.GetComponent<ActionBasedSnapTurnProvider>();
        snapTurn.enabled = false;
        base.OnSelectEntered(interactor);
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        GameObject locomotion = GameObject.Find("Locomotion System");
        ActionBasedSnapTurnProvider snapTurn = locomotion.GetComponent<ActionBasedSnapTurnProvider>();
        snapTurn.enabled = true;
        base.OnSelectExited(interactor);
    }
}
