using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Providers.Components.Gps;

public class DebugGpsProvider : GpsProviderBase {

    public double Latitude = 37.358102d;
    public double Longitude = -121.935913d;
    public double Height = 0;

    private GpsPosition _testGps = new GpsPosition();

    private void Awake()
    {
        _testGps.Latitude = Latitude;
        _testGps.Longitude = Longitude;
        _testGps.Height = Height;
    }


    public override  GpsPosition GetGPSPosition()
    {
        return _testGps;
    }

    public override  LocationServiceStatus GetLocationStatus()
    {
        return LocationServiceStatus.Running;   
    }

    public override  bool IsSupported()
    {
        return true;
    }

	public override void Destroy ()
	{
		
	}
}
