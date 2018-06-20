
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCoreInternal;

public class SturfeeArCoreManager : MonoBehaviour {

    private IEnumerator Start()
    {		
        while (Session.Status != SessionStatus.Tracking)
        {                        
            yield return null;
        }

        ProviderHelper.ArCoreSupported = true;
        GetComponent<SturfeeXRSession>().StartSession();
    }
}
