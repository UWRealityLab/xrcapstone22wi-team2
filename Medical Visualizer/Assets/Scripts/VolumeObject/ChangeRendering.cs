using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace MedicalVisualizer
{
    public class ChangeRendering : MonoBehaviour
    {
        VolumeRenderedObject volume;
        // Start is called before the first frame update
        void Start()
        {
            volume = GameObject.FindObjectOfType<VolumeRenderedObject>();
        }

        public void ToDirectVolume()
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("rpcToDirectVolume", RpcTarget.All);
            //CheckNull();
            //volume.SetRenderMode(RenderMode.DirectVolumeRendering);
        }

        public void ToMaxIntensity()
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("rpcToMaxIntensity", RpcTarget.All);
            //CheckNull();
            //volume.SetRenderMode(RenderMode.MaximumIntensityProjection);
        }

        public void ToIsoRendering()
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("rpcToIsoRendering", RpcTarget.All);
            //CheckNull();
            //volume.SetRenderMode(RenderMode.IsosurfaceRendering);
        }

        [PunRPC]
        void rpcToDirectVolume()
        {
            CheckNull();
            volume.SetRenderMode(RenderMode.DirectVolumeRendering);
        }

        [PunRPC]
        void rpcToMaxIntensity()
        {
            CheckNull();
            volume.SetRenderMode(RenderMode.MaximumIntensityProjection);
        }

        [PunRPC]
        void rpcToIsoRendering()
        {
            CheckNull();
            volume.SetRenderMode(RenderMode.IsosurfaceRendering);
        }


        // Update is called once per frame
        void Update()
        {
            CheckNull();
        }
        void CheckNull()
        {
            if (volume == null)
            {
                volume = GameObject.FindObjectOfType<VolumeRenderedObject>();
            }
        }
    }
}