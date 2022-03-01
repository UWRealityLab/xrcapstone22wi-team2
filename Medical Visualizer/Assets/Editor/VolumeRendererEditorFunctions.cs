using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.IO.Compression;

namespace MedicalVisualizer
{

    public class VolumeRendererEditorFunctions
    {

        [MenuItem("Volume Rendering/Load dataset/Load DICOM")]
        static void ShowDICOMImporter()
        {
            VolumeRenderedObject[] objects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            if (objects.Length != 1)
            {
                string dir = EditorUtility.OpenFolderPanel("Select a folder to load", "", "");
                openDicomFromDir(dir);
            }
        }

        [MenuItem("Volume Rendering/Load dataset/Load DICOM from attu")]
        static void ShowDICOMImporterFromAttu()
        {
            // e.g. C:/Users/WinJ/Documents/Git/481_V/Medical Visualizer/Assets
            string m_Path = Application.dataPath;
            Debug.Log("dataPath : " + m_Path);
            string filePath = m_Path + "/DICOM_FILES/dataset1.zip";
            string extractPath = m_Path + "/DICOM_FILES/dataset1";

            using WebClient client = new WebClient();
            try
            {

                // Download zip file
                // TODO: Change to be flexible
                client.DownloadFile("https://homes.cs.washington.edu/~winj/481V/dataset1_zip/dataset1.zip",
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
                            obj.transform.position = new Vector3(numVolumesCreated, 1, -18);
                            VolumeObjectFactory.SpawnCrossSectionPlane(obj, false);
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
}