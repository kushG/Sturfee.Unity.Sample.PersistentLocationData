using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;


public class MultiframeUI : MonoBehaviour {

    public GameObject LeftIndicator;
    public GameObject RightIndicator;

    private MultiframeManager _multiframeManager;
    private float _centerYaw;
    private Camera _xrCamera;

    private bool _isScanning;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnRequestAddedForMultiframe += OnRequestAddedForMultiframe;
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnRequestAddedForMultiframe -= OnRequestAddedForMultiframe;
    }

    private void Start()
    {
        _multiframeManager = GetComponent<MultiframeManager>();
        _xrCamera = GameObject.FindWithTag("XRCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if(_isScanning)
        {
            var gaze = GetNearestGazeTarget(_xrCamera);

            //check if this gaze target is in view
            if (gaze == null || CheckInView(_xrCamera, gaze.transform.position))
            {
                HideLeftIndicator();
                HideRightIndicator();
            }
            else
            //If not in view, show indicators
            {
                Vector3 targetDir = gaze.transform.position - _xrCamera.transform.position;
                float angle = Vector3.SignedAngle(targetDir, _xrCamera.transform.forward, Vector3.up);

                if (angle > 0)
                {
                    ShowLeftIndicator();
                }
                else
                {
                    ShowRightIndicator();
                }
            }

        }
    }

    public void ShowLeftIndicator(bool isBlinking = false)
    {
        LeftIndicator.SetActive(true);
        RightIndicator.SetActive(false);

        if(isBlinking)
        {
            StartCoroutine(BlinkGameObject(LeftIndicator));
        }
    }

    public void ShowRightIndicator(bool isBlinking = false)
    {
        RightIndicator.SetActive(true);
        LeftIndicator.SetActive(false);

        if (isBlinking)
        {
            StartCoroutine(BlinkGameObject(RightIndicator));
        }
    }

    public void HideLeftIndicator()
    {
        LeftIndicator.SetActive(false);
    }

    public void HideRightIndicator()
    {
        RightIndicator.SetActive(false);
    }

    private IEnumerator BlinkGameObject(GameObject go, float blinkAfter = 1)
    {
        while (go.activeSelf)
        {
            yield return new WaitForSeconds(blinkAfter);

            go.GetComponent<Image>().enabled = false;                

            yield return new WaitForSeconds(blinkAfter);

            go.GetComponent<Image>().enabled = true;
        }

        yield return null;            
    }

    private void OnRequestAddedForMultiframe(int requestNum, int requestLength)
    {
        if(requestNum == 1)
        {
            //Multiframe scan started
            _centerYaw = XRSessionManager.GetSession().GetXRCameraOrientation().eulerAngles.y;
            _isScanning = true;                
        }


        if (requestNum == requestLength)
        {
            //Multiframe scan complete
            _isScanning = false;
        }
    }

    private GameObject GetNearestGazeTarget(Camera camera)
    {
        float minAngle = Mathf.Infinity;
        GameObject nearestGazeTarget = null;

        foreach(GameObject gaze in _multiframeManager.GetGazeTargets())
        {
            if(gaze != null)
            {
                Vector3 targetDir = gaze.transform.position - camera.transform.position;
                float angle = Vector3.Angle(targetDir, camera.transform.forward);

                if(angle < minAngle)
                {
                    minAngle = angle;
                    nearestGazeTarget = gaze;
                }
            }
        }

        return nearestGazeTarget;
            
    }

    private bool CheckInView(Camera camera, Vector3 position)
    {
        var viewportPos = camera.WorldToViewportPoint(position);

        if( (viewportPos.x <= 1 && viewportPos.x >= 0) && (viewportPos.y <= 1 && viewportPos.y >= 0) && viewportPos.z >= 0 )
        {
            return true;
        }

        return false;            
    }
}
