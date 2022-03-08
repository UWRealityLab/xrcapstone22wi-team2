using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            CheckNull();
            volume.SetRenderMode(RenderMode.DirectVolumeRendering);
        }

        public void ToMaxIntensity()
        {
            CheckNull();
            volume.SetRenderMode(RenderMode.MaximumIntensityProjection);
        }

        public void ToIsoRendering()
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