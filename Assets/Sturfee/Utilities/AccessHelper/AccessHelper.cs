using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Features;
using Sturfee.Unity.XR.Core.Config;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Package.Utilities;

public class AccessHelper : MonoBehaviour
{

	public static Tierlevel CurrentTier
	{
		get
		{
            return _currentTier;			
		}
	}
    private static Tierlevel _currentTier = Tierlevel.Checking;

	private void Awake()
    {
        var accessToken = GetAccessToken();
        GetAvailableFeatures(accessToken);
    }

    private string GetAccessToken()
	{
		TextAsset configurationTextAsset = Resources.Load<TextAsset>(Paths.SturfeeResourcesRelative);
		SturfeeConfiguration config = JsonUtility.FromJson<SturfeeConfiguration>(configurationTextAsset.text);
		return config.AccessToken;
	}


    private void GetAvailableFeatures(string token)
    {        
		var tokenInfo =  SturfeeSubscriptionManager.GetSubscriptionInfo (token);
		_currentTier = (Tierlevel)tokenInfo.Tier;
    }
		
}

