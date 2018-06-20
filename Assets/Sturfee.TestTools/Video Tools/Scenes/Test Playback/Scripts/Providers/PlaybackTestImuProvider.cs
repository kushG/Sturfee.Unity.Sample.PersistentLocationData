using UnityEngine;
using Sturfee.Unity.XR.Providers.Components.Imu;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class PlaybackTestImuProvider : ImuProviderBase 
    {                
        private Data[] _sessionData;
        private Vector3 _initialPosition;

        private bool _isInitialPositionSet;
        private bool _isInitialPositionAfterLocalizationSet;

        private void Start()
        {
            TestPlaybackManager.Instance.OnDataReady += OnDataReady;
        }         

        public override Quaternion GetOrientation()
        {
            if (TestPlaybackManager.Instance.LocalizeWhilePlaying)
            {
                return _sessionData[TestPlaybackManager.Instance.DataIndex].unityOrientation;
            }
            else if (TestPlaybackManager.Instance.DataIndex < _sessionData.Length)
            {               
                return _sessionData [TestPlaybackManager.Instance.DataIndex].xrCameraOrientation;
            } 
            else 
            {
                Debug.LogError("Incorrect index");
                return Quaternion.identity;
            }
        }        

        public override Vector3 GetOffsetPosition()
        {

            if (TestPlaybackManager.Instance.DataIndex < _sessionData.Length)
            {                             
                //Loop
                if (TestPlaybackManager.Instance.DataIndex == 0)
                {
                    _isInitialPositionSet = false;
                    _isInitialPositionAfterLocalizationSet = false;
                }

                //Note : xrCameraPosition is addition of  GPSPosition(to local) and imu offsetPosition. We need to remove Gps(ToLocal) values to use correct IMU offset position

                //Removing GpsProvider's Gps(to Local) position
                if (!_isInitialPositionSet)
                {
                    _initialPosition = _sessionData [TestPlaybackManager.Instance.DataIndex].xrCameraPosition;
                    _isInitialPositionSet = true;
                    //Debug.Log("Initial position at start : " + _initialPosition);
                }

                //Removing Corrected Gps(to Local) position
                if (!_isInitialPositionAfterLocalizationSet && _sessionData [TestPlaybackManager.Instance.DataIndex].aiGpsLocation.Latitude != 0 && _sessionData [TestPlaybackManager.Instance.DataIndex].aiGpsLocation.Longitude != 0 )
                {                    
                    _initialPosition = _sessionData [TestPlaybackManager.Instance.DataIndex].xrCameraPosition;
                    _isInitialPositionAfterLocalizationSet = true;
                    //Debug.Log("Initial position after localization : " + _initialPosition);
                }
////				
                //Debug.Log(_sessionData[TestPlaybackManager.Instance.DataIndex].xrCameraPosition - _initialPosition);
                return (_sessionData [TestPlaybackManager.Instance.DataIndex].xrCameraPosition) - _initialPosition;
            } 
            else 
            {
                Debug.LogError("Incorrect index");
                return Vector3.zero;
            }
        }

        public override bool IsSupported()
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
