using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;

namespace Sturfee.Unity.XR.Package.Templates
{
    public class LocalizationManagerTemplate : MonoBehaviour
    {
		private bool _sessionIsReady;

        private void Awake()
        {
            // Register event handlers for Sturfee events

            // XR SESSION
            SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
            SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;

            // LOCALIZATION
            SturfeeEventManager.Instance.OnCoverageCheckComplete += OnCoverageCheck;
			SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
			SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;

            // SURFACE DETECTION (SERVER-SIDE RAYCAST)
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
            SturfeeEventManager.Instance.OnCoverageCheckComplete -= OnCoverageCheck;
            SturfeeEventManager.Instance.OnLocalizationComplete -= OnLocalizationComplete;
            SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;

            // SURFACE DETECTION (SERVER-SIDE RAYCAST)
            SturfeeEventManager.Instance.OnDetectSurfacePointComplete -= OnDetectSurfacePointComplete;
            SturfeeEventManager.Instance.OnDetectSurfacePointLoading -= OnDetectSurfacePointLoading;
            SturfeeEventManager.Instance.OnDetectSurfacePointFailed -= OnDetectSurfacePointFailed;
        }

        private void Update()
        {
            // Until the XR Session is ready, don't do anything
			if (!_sessionIsReady) 
			{
				return;
			}  

            if (AccessHelper.CurrentTier != Tierlevel.Tier3 && Input.GetMouseButtonDown(0)) 
            {
                XRSessionManager.GetSession().DetectSurfaceAtPoint(Input.mousePosition);
            }
        }
        

        #region XR SESSION
        private void OnSessionReady()
        {
			_sessionIsReady = true;

            // once the XR Sessions is ready, we need to make sure the user's location is in a supported coverage city/area
            // refer to: https://sturfee.com/faq.html#cities
            XRSessionManager.GetSession().CheckAlignment();
        }

        private void OnSessionFailed (string error)
        {
            // This event is fired when the XR Session could not be started
            // Possible issues:
            //   Permissions - GPS and Camera permissions are required
            Debug.LogError("Sturfee XR Session Intialization failed : " + error);
        }

        private void OnCoverageCheck(bool result)
        {
            // This event is fired after the server has responded to the 'CheckAlignment()' call

            if (result == false)
            {
                Debug.LogError("Localization is not available at this location");
                return;
            }

            Debug.Log("Localization Available");
        }
        #endregion


        #region LOCALIZATION
        public void LocalizeMe()
        {
            // This sends an image to the Sturfee servers
            // Once the server responds, the `OnLocalizationComplete` event will be fired
            XRSessionManager.GetSession().PerformLocalization();
        }

        private void OnLocalizationLoading ()
		{
            // This event is fired when the localization process has been started from the `PerformAlignment()` call
            Debug.Log("Localization in progress...");
		}

        private void OnLocalizationComplete (Core.Constants.Enums.AlignmentStatus status)
        {
            // This event is fired when the server responds to a localization request via `PerformAlignment()` call
            if (status == Core.Constants.Enums.AlignmentStatus.Done)
            {
                Debug.Log("Localization Complete");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.IndoorsError)
            {
                Debug.LogError("Localization Failed: Indoors Error");
            }
            else if (status == Core.Constants.Enums.AlignmentStatus.Error)
            {
                Debug.LogError("Localization Failed: ERROR");
            }
        }
        #endregion


        #region SURFACE DETECTION (SERVER-SIDE RAYCAST for Tier 1)
        private void OnDetectSurfacePointLoading()
        {
            // This event is fired when the server-side raycast process has been started from the `PerformLocalization()` call
            Debug.Log("Raycast in progress...");
        }

        private void OnDetectSurfacePointComplete(Core.Models.Location.GpsPosition gpsPosition, Vector3 normal)
		{
            // This event is fired in a successful response to a `DetectSurfaceAtPoint()` call
            if (AccessHelper.CurrentTier != Tierlevel.Tier3)
            {
                Debug.Log("Raycast complete");   
            }
		}

		private void OnDetectSurfacePointFailed ()
		{
            // This event is fired in an error response to a `DetectSurfaceAtPoint()` call
            Debug.LogError("Raycast failed"); 
		}
        #endregion
    }
}