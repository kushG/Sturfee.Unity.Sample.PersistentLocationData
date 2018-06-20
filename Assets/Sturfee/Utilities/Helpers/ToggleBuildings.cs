using System.Collections;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class ToggleBuildings : MonoBehaviour {

        [SerializeField]
        private GameObject ToggleButton;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
                {
                    return AccessHelper.CurrentTier != Tierlevel.Checking;
                });

            if (AccessHelper.CurrentTier == Tierlevel.Tier1)
            {
				ToggleButton.SetActive(false);
            }
        }
               
        public void ToggleDebugBuildings()
        {
            XRSessionManager.GetSession().ToggleDebugBuildings();
        }
    }
}