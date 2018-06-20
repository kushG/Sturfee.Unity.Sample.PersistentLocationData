using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using System;

namespace Sturfee.Unity.XR.Package.TestTools
{
    [Serializable]
	public class Data
	{
        //GPS
        public GpsPosition gpsLocation;

        //IMU
        public Quaternion unityOrientation;
        public Quaternion worldOrientation;
        public Vector3 imuOffsetPosition;

        //XR Session
		public Vector3 xrCameraPosition;
		public Quaternion xrCameraOrientation;
        public GpsPosition xrCameraLocation;
        		
		//Offset correction
		public Quaternion aiOffsetOrientation;
		public GpsPosition aiGpsLocation;

		public string timestamp;

        public SessionEvents sessionEvents;

        public bool PreRenderData;
	}


    [Serializable]

    public class SessionEvents
    {
        public bool alignmentStarted;
        public bool waitingForAlignment;
        public bool alignmentFinished;
        public bool alignmentFailed;
    }
}
