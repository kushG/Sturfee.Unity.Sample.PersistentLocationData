using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Providers.Components.Imu;
using Sturfee.Unity.XR.Core.Utilities;


public class SampleImuProvider : ImuProviderBase {

    private SampleManager _sampleManager;

    private void Start()
    {
        _sampleManager = GetComponent<SampleManager>();
    }

    public override Quaternion GetOrientation()
    {
        return OrientationUtils.WorldToUnity(_sampleManager.GetOrientationAtCurrentIndex());
    }

    public override Vector3 GetOffsetPosition()
    {
        return new Vector3();
    }

    public override bool IsSupported()
    {
        return true;
    }

    public override void Destroy()
    {

    }

    [System.Serializable]
    private class OrientationData
    {
        public double latitude;
        public double longitude;
        public string imName;
        public Quaternion quaternion;
    }
}
