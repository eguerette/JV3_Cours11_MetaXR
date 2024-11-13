using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeasureTapeFeature : MonoBehaviour
{

    [Range(0.005f, 0.005f)]
    [SerializeField] private float tapeWidth = 0.01f;
    [SerializeField] private OVRInput.Button tapeActionButton;
    [SerializeField] private Material tapeMaterial;

    [SerializeField] private GameObject measurmentInfoPrefab;
    [SerializeField] private Vector3 measurmentInfoControllerOffset = new (0, 0.45f, 0);

     [SerializeField] private Transform leftControllerTapeArea;
    [SerializeField] private Transform rightControllerTapeArea;

    private List<MeasuringTape> savedTapeLines = new();
    private TextMeshPro lastMeasurmentInfo;
    private LineRenderer lastTapeLinerenderer;
    private OVRInput.Controller? currentController;

    void Update()
    {
        HandleControllerActions(OVRInput.Controller.LTouch, leftControllerTapeArea);
        HandleControllerActions(OVRInput.Controller.RTouch, rightControllerTapeArea);

    }

    private void HandleControllerActions(OVRInput.Controller controller, Transform tapeArea) {
        if (currentController != controller && currentController != null) {

       
        if(OVRInput.GetDown(tapeActionButton, controller))
            currentController = controller;
            HandleDownAction(tapeArea);
        if(OVRInput.Get(tapeActionButton, controller)) 
            HandleHoldAction(tapeArea);
        if(OVRInput.GetUp(tapeActionButton, controller))
            currentController = null; 
            HandleUpAction(tapeArea);
        }
    }

    private void HandleDownAction(Transform tapeArea) {
        CreteNewTapeLine(tapeArea.position);
        AttachAndDetachMeasurmentInfo(tapeArea);
    }
    private void HandleHoldAction(Transform tapeArea) {
        lastTapeLinerenderer.SetPosition(1, tapeArea.position);
        AttachAndDetachMeasurmentInfo(tapeArea);
    }
    private void HandleUpAction(Transform tapeArea) {

        AttachAndDetachMeasurmentInfo(tapeArea, false);
    }

    private void CreteNewTapeLine(Vector3 initialPosition) {

        var newTapeLine = new GameObject($"TapeLine_{savedTapeLines.Count}",
        typeof(LineRenderer));

        lastTapeLinerenderer = newTapeLine.GetComponent<LineRenderer>();
        lastTapeLinerenderer.positionCount = 2;
        lastTapeLinerenderer.startWidth = tapeWidth;
        lastTapeLinerenderer.endWidth = tapeWidth;
        lastTapeLinerenderer.material = tapeMaterial;
        lastTapeLinerenderer.SetPosition(0, initialPosition);

        lastMeasurmentInfo = Instantiate(measurmentInfoPrefab, Vector3.zero, Quaternion.identity).GetComponent<TextMeshPro>();
        lastMeasurmentInfo.gameObject.SetActive(false);

        savedTapeLines.Add(new MeasuringTape
        {
            TapeLine = newTapeLine,
            TapeInfo = lastMeasurmentInfo
        });
    }

    private void AttachAndDetachMeasurmentInfo(Transform tapeArea, bool attachToController = true) {

        // attach to controller while we're doing a measurment
        if (attachToController) {
            
            lastMeasurmentInfo.gameObject.SetActive(true);
            lastMeasurmentInfo.transform.SetParent(tapeArea.transform.parent);
            lastMeasurmentInfo.transform.localPosition = measurmentInfoControllerOffset;
        }
        else //otherwise place the info between both points
        {
            lastTapeLinerenderer.transform.SetParent(lastTapeLinerenderer.transform);

            // mid point calculation

            Vector3 lineMidPoint = (lastTapeLinerenderer.GetPosition(0) + lastTapeLinerenderer.GetPosition(1)) / 2.0f;

            lastMeasurmentInfo.transform.position = lineMidPoint;
        }
    }
}

