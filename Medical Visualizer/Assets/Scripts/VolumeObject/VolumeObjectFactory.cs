using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

namespace MedicalVisualizer
{
    public class VolumeObjectFactory
    {
        public static VolumeRenderedObject CreateObject(VolumeDataset dataset)
        {
            GameObject outerObject = new GameObject("VolumeRenderedObject_" + dataset.datasetName);
            VolumeRenderedObject volObj = outerObject.AddComponent<VolumeRenderedObject>();

            //PhotonNetwork.Instantiate("VolumeRenderedObject_" + dataset.datasetName);
            GameObject meshContainer = GameObject.Instantiate((GameObject)Resources.Load("VolumeContainer"));
            meshContainer.transform.parent = outerObject.transform;
            meshContainer.transform.localScale = Vector3.one;
            meshContainer.transform.localPosition = Vector3.zero;
            meshContainer.transform.parent = outerObject.transform;
            outerObject.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

            MeshRenderer meshRenderer = meshContainer.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);
            volObj.meshRenderer = meshRenderer;
            volObj.dataset = dataset;

            const int noiseDimX = 512;
            const int noiseDimY = 512;
            Texture2D noiseTexture = NoiseTextureGenerator.GenerateNoiseTexture(noiseDimX, noiseDimY);

            TransferFunction tf = TransferFunctionDatabase.CreateTransferFunction();
            Texture2D tfTexture = tf.GetTexture();
            volObj.transferFunction = tf;

            TransferFunction2D tf2D = TransferFunctionDatabase.CreateTransferFunction2D();
            volObj.transferFunction2D = tf2D;

            meshRenderer.sharedMaterial.SetTexture("_DataTex", dataset.GetDataTexture());
            meshRenderer.sharedMaterial.SetTexture("_GradientTex", null);
            meshRenderer.sharedMaterial.SetTexture("_NoiseTex", noiseTexture);
            meshRenderer.sharedMaterial.SetTexture("_TFTex", tfTexture);

            meshRenderer.sharedMaterial.EnableKeyword("MODE_DVR");
            meshRenderer.sharedMaterial.DisableKeyword("MODE_MIP");
            meshRenderer.sharedMaterial.DisableKeyword("MODE_SURF");

            if (dataset.scaleX != 0.0f && dataset.scaleY != 0.0f && dataset.scaleZ != 0.0f)
            {
                float maxScale = Mathf.Max(dataset.scaleX, dataset.scaleY, dataset.scaleZ);
                volObj.transform.localScale = new Vector3(dataset.scaleX / maxScale, dataset.scaleY / maxScale, dataset.scaleZ / maxScale);
            }

            return volObj;
        }

        //        public static void SpawnCrossSectionPlane(VolumeRenderedObject volobj)
        //        {
        //            GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("CrossSectionPlane"));
        //            quad.transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
        //            CrossSectionPlane csplane = quad.gameObject.GetComponent<CrossSectionPlane>();
        //            csplane.targetObject = volobj;
        //            quad.transform.position = volobj.transform.position;
        //            csplane.sphere1 = GameObject.Instantiate((GameObject) Resources.Load("CrossSectionSphere"));
        //            csplane.sphere1.transform.position = new Vector3(0.5f, 0f, -19.5f);
        //            csplane.sphere2 = GameObject.Instantiate((GameObject) Resources.Load("CrossSectionSphere"));
        //            csplane.sphere2.transform.position = new Vector3(0f, 1f, -19.5f);
        //            csplane.sphere3 = GameObject.Instantiate((GameObject) Resources.Load("CrossSectionSphere"));
        //            csplane.sphere3.transform.position = new Vector3(-0.5f, 0f, -19.5f);


        //#if UNITY_EDITOR
        //            UnityEditor.Selection.objects = new UnityEngine.Object[] { quad };
        //#endif
        //        }

        //        public static void SpawnCutoutBox(VolumeRenderedObject volobj)
        //        {
        //            GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("CutoutBox"));
        //            obj.transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
        //            CutoutBox cbox = obj.gameObject.GetComponent<CutoutBox>();
        //            cbox.targetObject = volobj;
        //            obj.transform.position = volobj.transform.position;

        //#if UNITY_EDITOR
        //            UnityEditor.Selection.objects = new UnityEngine.Object[] { obj };
        //#endif
        //        }
        
        public static CrossSectionNetworkHelper SpawnCrossSectionPlane(VolumeRenderedObject volobj, bool isOwner)
        {
            //PhotonNetwork.Instantiate("CrossSectionPlane", volobj.transform.position, Quaternion.Euler(270.0f, 0.0f, 0.0f));
            GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("CrossSectionPlane"));
            quad.transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
            CrossSectionPlane csplane = quad.gameObject.GetComponent<CrossSectionPlane>();
            csplane.targetObject = volobj;
          
            quad.transform.position = volobj.transform.position;
            int id1 = 0, id2 = 0, id3 = 0;
            if (isOwner)
            {
                csplane.sphere1 = PhotonNetwork.Instantiate("CrossSectionSphere", new Vector3(0.5f, 0f, -19.5f), Quaternion.identity);
                csplane.sphere2 = PhotonNetwork.Instantiate("CrossSectionSphere", new Vector3(0f, 1f, -19.5f), Quaternion.identity);
                csplane.sphere3 = PhotonNetwork.Instantiate("CrossSectionSphere", new Vector3(-0.5f, 0f, -19.5f), Quaternion.identity);
                id1 = csplane.sphere1.GetPhotonView().ViewID;
                id2 = csplane.sphere2.GetPhotonView().ViewID;
                id3 = csplane.sphere3.GetPhotonView().ViewID;
            }
#if UNITY_EDITOR
            UnityEditor.Selection.objects = new UnityEngine.Object[] { quad };
#endif
            return new CrossSectionNetworkHelper(csplane, id1, id2, id3);
        }

        public static void SpawnCutoutBox(VolumeRenderedObject volobj, bool isOwner)
        {
            if (isOwner)
            {
                GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("CutoutBox"));
                obj.transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
                CutoutBox cbox = obj.gameObject.GetComponent<CutoutBox>();
                cbox.targetObject = volobj;
                obj.transform.position = volobj.transform.position;

#if UNITY_EDITOR
                UnityEditor.Selection.objects = new UnityEngine.Object[] { obj };
#endif
            }
        }

        public class CrossSectionNetworkHelper
        {
            public CrossSectionPlane csplane;
            public int id_1;
            public int id_2;
            public int id_3;

            public CrossSectionNetworkHelper(CrossSectionPlane cs, int id1, int id2, int id3)
            {
                csplane = cs;
                id_1 = id1;
                id_2 = id2;
                id_3 = id3;
            }

        }
    }
}