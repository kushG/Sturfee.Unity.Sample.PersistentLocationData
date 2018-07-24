using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

public class SingleFrameManager : MonoBehaviour, IScanManager {

    [SerializeField]
    private GameObject _scanAnimation;


	public void OnScanButtonClick()
    {
        XRSessionManager.GetSession().PerformLocalization();
    }


    public void PlayScanAnimation()
    {
        if (_scanAnimation != null)
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
}
