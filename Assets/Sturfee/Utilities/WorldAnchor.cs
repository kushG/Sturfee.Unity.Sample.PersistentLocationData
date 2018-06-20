using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class WorldAnchor : MonoBehaviour
    {
        public GpsPosition GpsPosition;

        [Tooltip("Update position in the update loop (true) or only when the XRSession is ready (false)")]
        public bool Dynamic = false;

        private void Awake()
        {
            SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        }

        private void LateUpdate()
        {
            if (Dynamic)
            {
                transform.position = XRSessionManager.GetSession().GpsToLocalPosition(GpsPosition);
            }
        }

        private void OnSessionReady()
        {
            transform.position = XRSessionManager.GetSession().GpsToLocalPosition(GpsPosition);
        }
    }
}