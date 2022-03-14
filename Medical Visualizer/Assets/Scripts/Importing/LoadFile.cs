using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.IO.Compression;
using MedicalVisualizer;
using UnityEngine.XR;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class LoadFile : MonoBehaviour
{
    [SerializeField]
    public WhiteboardManager wm;

    private bool leftButtonPressed;
    [SerializeField]
    private XRNode xrNodeLeft = XRNode.LeftHand;
    private List<InputDevice> leftDevices = new List<InputDevice>();
    private InputDevice leftDevice;

    private bool rightButtonPressed;
    [SerializeField]
    private XRNode xrNodeRight = XRNode.RightHand;
    private List<InputDevice> rightDevices = new List<InputDevice>();
    private InputDevice rightDevice;


    private string filePath1 = "/dicom1.zip";
    private string extractPath1 = "/dicom1";
    private string downloadPath1 = "https://firebasestorage.googleapis.com/v0/b/medical-imaging-fa58c.appspot.com/o/datasets%2Fdicom1.zip?alt=media&token=b1e82100-7f13-49e9-8b51-f92acbcc9a64";


    private string filePath2 = "/dicom2.zip";
    private string extractPath2 = "/dicom2";
    private string downloadPath2 = "https://firebasestorage.googleapis.com/v0/b/medical-imaging-fa58c.appspot.com/o/datasets%2Fdicom2.zip?alt=media&token=c886d084-e98c-4670-8b2b-bae19547269d";

    private VolumeObjectFactory.CrossSectionNetworkHelper cshelper;
    private CrossSectionPlane csplane;

    private int file = 1;
    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNodeLeft, leftDevices);
        leftDevice = leftDevices.FirstOrDefault();
        InputDevices.GetDevicesAtXRNode(xrNodeRight, rightDevices);
        rightDevice = rightDevices.FirstOrDefault();
    }

    private void OnEnable()
    {
        if (!leftDevice.isValid)
        {
            GetDevice();
        }
        if (!rightDevice.isValid)
        {
            GetDevice();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //loadFromAttu(filePath1, extractPath1, downloadPath1);
        //file = 0;
        disableLoadingInfo();
    }

    void enableLoadingInfo()
    {
        GameObject obj = GameObject.Find("LoadingInfo");
        foreach (Image img in obj.GetComponentsInChildren<Image>())
        {
            img.enabled = true;
        }
        foreach (Text t in obj.GetComponentsInChildren<Text>())
        {
            t.enabled = true;
        }
    }

    void disableLoadingInfo()
    {
        GameObject obj = GameObject.Find("LoadingInfo");
        foreach (Image img in obj.GetComponentsInChildren<Image>())
        {
            img.enabled = false;
        }
        foreach (Text t in obj.GetComponentsInChildren<Text>())
        {
            t.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (!leftDevice.isValid || !rightDevice.isValid)
        //{
        //    GetDevice();
        //}
        //bool leftPrimaryButton = false;
        //if (leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out leftPrimaryButton) && leftPrimaryButton && !leftButtonPressed)
        //{
        //    leftButtonPressed = true;
        //    Debug.Log("left pressed");
        //}
        //else if (!leftPrimaryButton && leftButtonPressed)
        //{
        //    leftButtonPressed = false;
        //    Debug.Log("left released");
        //    if (file != 0)
        //    {
        //        file = 0;
        //        deleteObjects();
        //        loadFromAttu(filePath1, extractPath1, downloadPath1);
        //    }
        //}

        //bool rightPrimaryButton = false;
        //if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButton) && rightPrimaryButton && !rightButtonPressed)
        //{
        //    rightButtonPressed = true;
        //    Debug.Log("right pressed");
        //}
        //else if (!rightPrimaryButton && rightButtonPressed)
        //{
        //    rightButtonPressed = false;
        //    Debug.Log("right released");
        //    if (file != 1)
        //    {
        //        file = 1;
        //        deleteObjects();
        //        loadFromAttu(filePath2, extractPath2, downloadPath2);
        //    }
        //}
    }

    public void deleteObjects()
    {
        MedicalVisualizer.VolumeRenderedObject[] vro = GameObject.FindObjectsOfType<VolumeRenderedObject>();
        foreach (VolumeRenderedObject o in vro)
        {
            o.DestroyGameObject();
        }
        MedicalVisualizer.CrossSectionPlane[] csp = GameObject.FindObjectsOfType<MedicalVisualizer.CrossSectionPlane>();
        foreach (CrossSectionPlane o in csp)
        {
            o.DestroyGameObject();
        }
        CrossSectionSphere[] css = GameObject.FindObjectsOfType<CrossSectionSphere>();
        foreach (CrossSectionSphere o in css)
        {
            if (o.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(o.GetComponent<PhotonView>());
            }
        }
    }

    public void loadSampleDicom_1()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("rpcLoadSampleDicom", RpcTarget.Others, filePath1, extractPath1, downloadPath1);
        deleteObjects();
        enableLoadingInfo();
        loadFromAttu(filePath1, extractPath1, downloadPath1, true);
        disableLoadingInfo();
    }

    public void loadSampleDicom_2()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("rpcLoadSampleDicom", RpcTarget.Others, filePath2, extractPath2, downloadPath2);
        deleteObjects();
        enableLoadingInfo();
        loadFromAttu(filePath2, extractPath2, downloadPath2, true);
        disableLoadingInfo();
    }


    [PunRPC]
    void rpcLoadSampleDicom(string filePath, string extractPath, string downloadPath, PhotonMessageInfo info)
    {
        Debug.Log("RPC is called to load DICOM");
        deleteObjects();
        enableLoadingInfo();
        loadFromAttu(filePath, extractPath, downloadPath, false);
        disableLoadingInfo();
        Debug.LogFormat("Got a message from {0} to load dicom", info.Sender);
        PhotonView.Get(this).RPC("finishedLoading", info.Sender);
    }

    [PunRPC]
    void finishedLoading(PhotonMessageInfo info)
    {
        Debug.LogFormat("Got a message from {0} they finished loading", info.Sender);
        if (cshelper.id_1 == 0 && cshelper.id_2 == 0 && cshelper.id_3 == 0)
            return;

        PhotonView.Get(this).RPC("addCrossSectionSphere", RpcTarget.Others, cshelper.id_1, cshelper.id_2, cshelper.id_3);
    }

    [PunRPC]
    void addCrossSectionSphere(int id1, int id2, int id3, PhotonMessageInfo info)
    {
        Debug.LogFormat("Got a message from {0} to add cross section sphere", info.Sender);
        if (csplane == null)
            return;

        csplane.sphere1 = PhotonView.Find(id1).gameObject;
        csplane.sphere2 = PhotonView.Find(id2).gameObject;
        csplane.sphere3 = PhotonView.Find(id3).gameObject;
        wm.generateObjects();
    }

    void loadFromAttu(string fp, string ep, string dp, bool isOwner)
    {
        // e.g. C:/Users/WinJ/Documents/Git/481_V/Medical Visualizer/Assets
        string m_Path = Application.dataPath;
        Debug.Log("dataPath : " + m_Path);
        string filePath = m_Path + fp;
        string extractPath = m_Path + ep;

        using WebClient client = new WebClient();
        try
        {
            Debug.Log("trying to download");
            // Download zip file
            // TODO: Change to be flexible
            client.DownloadFile(dp,
                filePath);
            Debug.Log("Successfully downloaded to: " + filePath);

            // unzip
            ZipFile.ExtractToDirectory(filePath, extractPath);
            Debug.Log("Successfully extracted to: " + extractPath);
        }
        catch (IOException) { }
        // call api
        openDicomFromDir(extractPath, isOwner);
        if (isOwner)
        {
            wm.generateObjects();
        }
    }
    void openDicomFromDir(string dir, bool isOwner)
    {
        if (Directory.Exists(dir))
        {
            bool recursive = true;

            // Read all files
            IEnumerable<string> fileCandidates = Directory.EnumerateFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(p => p.EndsWith(".dcm", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicom", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicm", StringComparison.InvariantCultureIgnoreCase));

            if (!fileCandidates.Any())
            {
                //#if UNITY_EDITOR
                //                    if (UnityEditor.EditorUtility.DisplayDialog("Could not find any DICOM files",
                //                        $"Failed to find any files with DICOM file extension.{Environment.NewLine}Do you want to include files without DICOM file extension?", "Yes", "No"))
                //                    {
                //                        fileCandidates = Directory.EnumerateFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                //                    }
                //#endif
            }

            if (fileCandidates.Any())
            {
                DICOMImporter importer = new DICOMImporter(fileCandidates, Path.GetFileName(dir));
                List<DICOMImporter.DICOMSeries> seriesList = importer.LoadDICOMSeries();
                float numVolumesCreated = 0;

                foreach (DICOMImporter.DICOMSeries series in seriesList)
                {
                    VolumeDataset dataset = importer.ImportDICOMSeries(series);
                    if (dataset != null)
                    {
                        //if (EditorPrefs.GetBool("DownscaleDatasetPrompt"))
                        //{
                        //    if (EditorUtility.DisplayDialog("Optional DownScaling",
                        //        $"Do you want to downscale the dataset? The dataset's dimension is: {dataset.dimX} x {dataset.dimY} x {dataset.dimZ}", "Yes", "No"))
                        //    {
                        //        dataset.DownScaleData();
                        //    }
                        //}
                        VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                        obj.transform.position = new Vector3(numVolumesCreated, 1, -18);
                        VolumeObjectFactory.CrossSectionNetworkHelper cshelpertmp = VolumeObjectFactory.SpawnCrossSectionPlane(obj, isOwner);
                        csplane = cshelpertmp.csplane;
                        cshelper = cshelpertmp;
                        numVolumesCreated++;
                        //PhotonView.Get(this).RPC()
                    }
                }
            }
            else
                Debug.LogError("Could not find any DICOM files to import.");
        }
        else
        {
            Debug.LogError("Directory doesn't exist: " + dir);
        }
    }
}