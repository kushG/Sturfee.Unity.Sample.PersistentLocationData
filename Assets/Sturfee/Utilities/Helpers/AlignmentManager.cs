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
        public GameObject ScanAnimation;
        public GameObject QuitButton;
        public Canvas LandscapeCanvas;
        public Canvas PortraitCanvas;

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

        private void Update()
        {
			if (!_sessionIsReady) 
			{
				return;
			}

            if (ScreenOrientationHelper.Instance.OrientationChanged || (!LandscapeCanvas.isActiveAndEnabled && !PortraitCanvas.isActiveAndEnabled))
            {
                UpdateOrientation();
            }

            if (Input.GetMouseButtonDown(0)) 
            {
                XRSessionManager.GetSession().DetectSurfaceAtPoint(Input.mousePosition);               
            }
        }

        #region XR SESSION
        private void OnSessionReady()
        {
			_sessionIsReady = true;

            //Uncomment this once portrait mode is supported
            //UpdateOrientation();

            XRSessionManager.GetSession().CheckCoverage();
        }

        private void OnSessionFailed (string error)
        {
            ToastManager.Instance.ShowToast("Session Intialization failed : " + error);
        }

        private void OnCoverageCheckComplete (bool result)
        {
            if(result == false)
            {
                ToastManager.Instance.ShowToast("Localization not available at this location");
                return;
            }

			LandscapeCanvas.gameObject.SetActive(true);

            Debug.Log("Localization available");
        }
        #endregion

        #region LOCALIZATION
        public void Capture()
        {
            XRSessionManager.GetSession().PerformLocalization();
        }

        private void OnLocalizationLoading()
        {
            //ToastManager.Instance.ShowToast("Alignment in progress...");
            ScanAnimation.SetActive(true);
        }

        private void OnLocalizationComplete (Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
        {			
            QuitButton.SetActive(true);

            if (status == Core.Constants.Enums.AlignmentStatus.Done)
            {
                Debug.Log ("Localization Complete");
                ToastManager.Instance.ShowToast("Tap anywhere to place planes");
                ScanAnimation.SetActive(false);
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.IndoorsError)
            {
                ToastManager.Instance.ShowToastTimed("Localization Failed: Indoors Error");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.Error)
            {
                ToastManager.Instance.ShowToastTimed("Localization failed");
            }
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

        private void UpdateOrientation()
        {
            // un-comment this when portrait mode is supported
            //if (LandscapeCanvas != null && PortraitCanvas != null)
            //{
            //    if (Screen.orientation == ScreenOrientation.Landscape) {
            //        LandscapeCanvas.gameObject.SetActive (true);
            //        PortraitCanvas.gameObject.SetActive (false);
            //    } else if (Screen.orientation == ScreenOrientation.Portrait) {
            //        LandscapeCanvas.gameObject.SetActive (false);
            //        PortraitCanvas.gameObject.SetActive (true);
            //    }
            //}
        }
    }
}