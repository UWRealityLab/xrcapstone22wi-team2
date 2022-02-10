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

public class LoadFile : MonoBehaviour
{
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


    private string filePath1 = "/dataset1.zip";
    private string extractPath1 = "/dataset1";
    private string downloadPath1 = "https://homes.cs.washington.edu/~winj/481V/dataset1_zip/dataset1.zip";


    private string filePath2 = "/dataset1_1.zip";
    private string extractPath2 = "/dataset1_1";
    private string downloadPath2 = "https://homes.cs.washington.edu/~winj/481V/dataset1_zip/dataset1_1.zip";

    private int file;
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
        loadFromAttu(filePath1, extractPath1, downloadPath1);
        file = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftDevice.isValid || !rightDevice.isValid)
        {
            GetDevice();
        }
        bool leftPrimaryButton = false;
        if (leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out leftPrimaryButton) && leftPrimaryButton && !leftButtonPressed)
        {
            leftButtonPressed = true;
            Debug.Log("left pressed");
        }
        else if (!leftPrimaryButton && leftButtonPressed)
        {
            leftButtonPressed = false;
            Debug.Log("left released");
            if (file != 0)
            {
                deleteObjects();
                loadFromAttu(filePath1, extractPath1, downloadPath1);
                file = 0;
            }
        }

        bool rightPrimaryButton = false;
        if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButton) && rightPrimaryButton && !rightButtonPressed)
        {
            rightButtonPressed = true;
            Debug.Log("right pressed");
        }
        else if (!rightPrimaryButton && rightButtonPressed)
        {
            rightButtonPressed = false;
            Debug.Log("right released");
            if (file != 1)
            {
                deleteObjects();
                loadFromAttu(filePath2, extractPath2, downloadPath2);
                file = 1;
            }
        }
    }

    static void deleteObjects()
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
            o.DestroyGameObject();
        }
    }

    static void loadFromAttu(string fp, string ep, string dp)
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
        openDicomFromDir(extractPath);
    }
    static void openDicomFromDir(string dir)
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
                        obj.transform.position = new Vector3(numVolumesCreated, 1, 1);
                        VolumeObjectFactory.SpawnCrossSectionPlane(obj);
                        numVolumesCreated++;
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
