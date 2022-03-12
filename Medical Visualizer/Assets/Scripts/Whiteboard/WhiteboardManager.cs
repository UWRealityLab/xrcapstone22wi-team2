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

public class WhiteboardManager: MonoBehaviour
{
    private bool rightButtonPressed;
    [SerializeField]
    private XRNode xrNodeRight = XRNode.RightHand;
    private List<InputDevice> rightDevices = new List<InputDevice>();
    private InputDevice rightDevice;

    private bool whiteboardGenerated;


    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNodeRight, rightDevices);
        rightDevice = rightDevices.FirstOrDefault();
    }

    private void OnEnable()
    {
        if (!rightDevice.isValid)
        {
            GetDevice();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        whiteboardGenerated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rightDevice.isValid)
        {
            GetDevice();
        }
        bool rightPrimaryButton = false;
        if (rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out rightPrimaryButton) && rightPrimaryButton && !rightButtonPressed)
        {
            rightButtonPressed = true;
        } else if (!rightPrimaryButton && rightButtonPressed)
        {
            rightButtonPressed = false;
            if (whiteboardGenerated)
            {
                DestroyObjects();

            } else
            {
                GenerateWhiteboard();
                GenerateMarker();
            }
            whiteboardGenerated = !whiteboardGenerated;
        }
    }

    void GenerateWhiteboard()
    {
        GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("Whiteboard"));
        quad.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Whiteboard wb = quad.gameObject.GetComponent<Whiteboard>();

        CrossSectionSphere[] css = GameObject.FindObjectsOfType<CrossSectionSphere>();
        wb.sphere1 = css[0];
        wb.sphere2 = css[1];
        wb.sphere3 = css[2];
    }

    void GenerateMarker()
    {
        GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("Marker"));
        quad.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        WhiteboardMarker wm = quad.gameObject.GetComponent<WhiteboardMarker>();
        wm.hand = GameObject.FindGameObjectWithTag("RightHand");
    }

    void DestroyObjects()
    {
        Whiteboard[] ws = GameObject.FindObjectsOfType<Whiteboard>();
        foreach (Whiteboard wb in ws)
        {
            wb.DestroyGameObject();
        }

        WhiteboardMarker[] ms = GameObject.FindObjectsOfType<WhiteboardMarker>();
        foreach (WhiteboardMarker m in ms)
        {
            m.DestroyGameObject();
        }
    }

}
