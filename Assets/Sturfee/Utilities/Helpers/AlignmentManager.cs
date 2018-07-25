using System.Collections;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class AlignmentManager : MonoBehaviour
    {
        [Header("AR Objects")]
        public GameObject HitObject;
        public bool ShowGrid;

        [Header("UI")]
        public GameObject ScanButton;
        public GameObject QuitButton;

        [Header("Scan Type")]
        public bool UseMultiframeLocalization;
        public bool ScanAgain;

        [Header("Coverage")]
        public bool CheckCoverageOnStart;

        private IScanManager _scanManager;

        private bool _sessionIsReady;
        private GameObject _hitObject;

        private void Awake()
        {            
            SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
            SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;
            SturfeeEventManager.Instance.OnCoverageCheckComplete += OnCoverageCheckComplete;
            SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
            SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
            SturfeeEventManager.Instance.OnDetectSurfacePointComplete += OnDetectSurfacePointComplete;
            SturfeeEventManager.Instance.OnDetectSurfacePointLoading += OnDetectSurfacePointLoading;
            SturfeeEventManager.Instance.OnDetectSurfacePointFailed += OnDetectSurfacePointFailed;
        }

        private void OnDestroy()
        {
            // *IMPORTANT* Unregister event handlers for Sturfee events

            // XR SESSION
            SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
            SturfeeEventManager.Instance.OnSessionFailed -= OnSessionFailed;

            // LOCALIZATION
            SturfeeEventManager.Instance.OnCoverageCheckComplete -= OnCoverageCheckComplete;
            SturfeeEventManager.Instance.OnLocalizationComplete -= OnLocalizationComplete;
            SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;

            // SURFACE DETECTION (SERVER-SIDE RAYCAST)
            SturfeeEventManager.Instance.OnDetectSurfacePointComplete -= OnDetectSurfacePointComplete;
            SturfeeEventManager.Instance.OnDetectSurfacePointLoading -= OnDetectSurfacePointLoading;
            SturfeeEventManager.Instance.OnDetectSurfacePointFailed -= OnDetectSurfacePointFailed;
        }

        private void Start()
        {
            if (QuitButton != null)
            {
                QuitButton.SetActive(true);
            }

            if(UseMultiframeLocalization)
            {
                _scanManager = GetComponentInChildren<MultiframeManager>(true);
                //_scanManager = GetComponentInChildren<MFSampleSceneRecord>(true);
            }
            else
            {
                _scanManager = GetComponentInChildren<SingleFrameManager>(true);
            }
        }

        private void Update()
        {
            if (!_sessionIsReady) 
            {
                return;
            }
                
            if (Input.GetMouseButtonDown(0)) 
            {
                XRSessionManager.GetSession().DetectSurfaceAtPoint(Input.mousePosition);               
            }
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        #region XR SESSION
        private void OnSessionReady()
        {
            _sessionIsReady = true;

            if (!CheckCoverageOnStart)
            {
                ScanButton.SetActive(true);
            }
            else
            {
                XRSessionManager.GetSession().CheckCoverage();
            }
        }

        private void OnSessionFailed (string error)
        {
            ToastManager.Instance.ShowToast("Session Intialization failed : " + error);
        }

        private void OnCoverageCheckComplete (bool result)
        {
            if(result == false)
            {
                ToastManager.Instance.ShowToastTimed("Localization not available at this location. Localization calls won't work", 5);
                return;
            }

            Debug.Log("Localization available");

            if(CheckCoverageOnStart)
            {
                ScanButton.SetActive(true);
            }
        }
        #endregion

        #region LOCALIZATION
        public void Capture()
        {            
            _scanManager.OnScanButtonClick();
        }

        private void OnLocalizationLoading()
        {
            _scanManager.PlayScanAnimation();
        }

        private void OnLocalizationComplete (Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
        {
            _scanManager.StopScanAnimation();

            if(ScanAgain)
            {
                ScanButton.SetActive(true);
            }

            if (status == Core.Constants.Enums.AlignmentStatus.Done)
            {
                Debug.Log ("Localization Complete");
                ToastManager.Instance.ShowToastTimed("Tap anywhere to place planes");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.OutOfCoverage)
            {
                ToastManager.Instance.ShowToastTimed("Localization not available at this location");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.IndoorsError)
            {
                ToastManager.Instance.ShowToastTimed("Localization Failed: Indoors Error");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.RequestError)
            {
                ToastManager.Instance.ShowToastTimed("Localization Failed: Request Error");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.Error)
            {
                ToastManager.Instance.ShowToastTimed("Localization failed");
            }

            StartCoroutine(Rescan() );
        }
        #endregion

        #region SURFACE DETECTION (SERVER-SIDE RAYCAST for Tier 1)
        private void OnDetectSurfacePointLoading()
        {
            //ToastManager.Instance.ShowToast("Raycast in progress...");
        }

        private void OnDetectSurfacePointComplete (Core.Models.Location.GpsPosition gpsPosition, Vector3 normal)
        {
            if (ShowGrid)
            {
                _hitObject = Instantiate(HitObject, XRSessionManager.GetSession().GpsToLocalPosition(gpsPosition) + (0.1f * normal), Quaternion.LookRotation(normal));
            }

            if (AccessHelper.CurrentTier != Tierlevel.Tier3)
            {
                ToastManager.Instance.ShowToastTimed("Raycast complete");   
            }
        }

        private void OnDetectSurfacePointFailed()
        {
            ToastManager.Instance.ShowToastTimed("Raycast failed");
        }
        #endregion

        private IEnumerator Rescan()
        {
            //wait for couple of seconds
            yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(1);

            if (ScanAgain)
            {
                ScanButton.SetActive(true);
            }
        }
    }
}