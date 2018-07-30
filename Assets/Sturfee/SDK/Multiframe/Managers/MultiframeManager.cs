using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Exceptions;


public class MultiframeManager : MonoBehaviour, IScanManager {

    //Note: Do not change these values for release 1.0.0
    public static readonly int  Angle = 50;
    public static readonly int TargetCount = 3;

    [Header("UI")]
    [SerializeField]
    private GameObject _scanAnimation;
    [SerializeField]
    private GameObject _gazeTarget;
    [SerializeField]
    private GameObject _cursor;
    [SerializeField]
    private GameObject _scanFX;
    [SerializeField]
    private GameObject _resetButton;


    private List<GameObject> _gazeTargets;

    private int _multiframeRequestId;
    private int _targetCount;
    private bool _isScanning;
    private bool _resetCalled;


    private void Start()
    {
        _targetCount = TargetCount - 1;
    }

    public void OnScanButtonClick()
    {
        _isScanning = true;

        StartCoroutine(MultiframeCallAsync());
    }

    public void PlayScanAnimation()
    {
        if(_scanAnimation != null)
        {
            _scanAnimation.SetActive(true);
        }
    }

    public void StopScanAnimation()
    {
        if (_scanAnimation != null)
        {
            _scanAnimation.SetActive(false);
        }
    }

    public void ResetMultiframe()
    {
        _resetCalled = true;
        FindObjectOfType<Sturfee.Unity.XR.Package.Utilities.AlignmentManager>().ScanButton.SetActive(true);
    }

    public List<GameObject> GetGazeTargets()
    {
        return _gazeTargets;
    }

    private IEnumerator MultiframeCallAsync()
    {
        //Setup variables for starting multiframe requests
        SetupMultiframe();

        //Create gaze targets
        CreateGazeTargets();

        //wait till all the requests are complete
        while (_multiframeRequestId != TargetCount)
        {
            yield return null;
        }

        _cursor.SetActive(false);
        _scanFX.SetActive(false);
        _resetButton.SetActive(false);

        _isScanning = false;
    }

    private void SetupMultiframe()
    {
        _resetCalled = false;

        _gazeTargets = new List<GameObject>();

        _cursor.SetActive(true);
        _scanFX.SetActive(true);
        _resetButton.SetActive(true);

        _multiframeRequestId = 0;

        AddToMultiframeLocalizationCall();
    }

    private void CreateGazeTargets()
    {
        int even = 0;
        int odd = 0;

        //Depending on the count, create equal number of targets on either sides
        for (int i = 0; i < _targetCount; i++)
        {
            int j;
            Vector3 dir;

            if (i % 2 == 0)
            {
                j = -(++even);      //LEFT
            }
            else
            {
                j = ++odd;          //RIGHT
            }

            var gaze = Instantiate(_gazeTarget);
            dir = Quaternion.AngleAxis(Angle * j, Vector3.up) * (XRSessionManager.GetSession().GetXRCameraOrientation() * Vector3.forward);
            gaze.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() + (dir * 10.0f);
            gaze.transform.LookAt(GameObject.FindGameObjectWithTag("XRCamera").transform);

            gaze.name = "Gaze target (" + (Angle * j) + ")";

            _gazeTargets.Add(gaze);

            StartCoroutine(CheckGazeTargetHit(gaze));
        }
    }
   
    private IEnumerator CheckGazeTargetHit(GameObject target)
    {
        RaycastHit hit;
        bool done = false;

        while (!done && !_resetCalled)
        {
            Camera xrCamera = GameObject.FindGameObjectWithTag("XRCamera").GetComponent<Camera>();
            //Debug.DrawRay(xrCamera.transform.position, xrCamera.transform.forward * 1000, Color.green, 2000);

            //TODO: Exclude building/terrain layers
            if (Physics.Raycast(xrCamera.transform.position, xrCamera.transform.forward, out hit, Mathf.Infinity))//, ~LayerMask.NameToLayer("Multiframe UI")))
            {
                if (hit.transform.gameObject == target)
                {                    
                    done = true;
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }

        Destroy(target);

        AddToMultiframeLocalizationCall();
    
    }

    private void AddToMultiframeLocalizationCall()
    {
        _multiframeRequestId++;

        try
        {
            //If reset is called, send -1 otherwise the count
            XRSessionManager.GetSession().PerformLocalization((_resetCalled) ? -1 : TargetCount);
        }


        catch(InvalidRequestLengthException e)
        {
            Debug.Log(e.Message);
            ToastManager.Instance.ShowToastTimed("[RequestError] :: Request Length is invalid", 5);
        }

        catch (PitchRequestException e)
        {
            Debug.Log(e.Message);
            ToastManager.Instance.ShowToastTimed("[RequestError] :: Please Look straight up while scanning", 5);
        }

        catch (YawRequestException e)
        {
            Debug.Log(e.Message);
            ToastManager.Instance.ShowToastTimed("[RequestError] :: Yaw difference between frames is incorrect", 5);
        }
        catch (UserMovedRequestException e)
        {
            Debug.Log(e.Message);
            ToastManager.Instance.ShowToastTimed("[RequestError] :: Please do not move while scannning.", 5);
        }


    }

   
}
