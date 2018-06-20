using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Providers.Components.Gps;


namespace Sturfee.Unity.XR.Package.TestTools
{
    public class PlaybackTestGpsProvider : GpsProviderBase
    {
        
        private Data[] _sessionData;

		private void Start()
		{
            TestPlaybackManager.Instance.OnDataReady += OnDataReady;
		}

		public override GpsPosition GetGPSPosition ()
		{
            if (TestPlaybackManager.Instance.LocalizationDone)
            {
                // use corrected GPS
                return _sessionData[TestPlaybackManager.Instance.DataIndex].gpsLocation;
                //return _sessionData[TestPlaybackManager.Instance.DataIndex].aiGpsLocation;
            }
            else if (TestPlaybackManager.Instance.DataIndex < _sessionData.Length)
            {                               
				return _sessionData [TestPlaybackManager.Instance.DataIndex].gpsLocation;			
            } 
            else 
            {
                Debug.LogError("Incorrect index");
                return null;
            }
		}

		public override LocationServiceStatus GetLocationStatus ()
		{
            return LocationServiceStatus.Running;
		}

		public override bool IsSupported ()
		{
			return true;
		}            

        public void OnDataReady(Data[] sessionData)
        {            
            _sessionData = sessionData;
        }

		public override void Destroy ()
		{
			
		}

	}
}