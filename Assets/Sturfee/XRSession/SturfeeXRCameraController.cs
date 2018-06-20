using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

public class SturfeeXRCameraController : MonoBehaviour {

    private void Update()
    {
        if (XRSessionManager.GetSession() == null)
        {
            return;
        }

        transform.position = XRSessionManager.GetSession().GetXRCameraPosition();
        transform.rotation = XRSessionManager.GetSession().GetXRCameraOrientation();
    }
}
