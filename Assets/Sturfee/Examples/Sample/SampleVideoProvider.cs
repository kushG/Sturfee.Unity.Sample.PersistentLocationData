using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Providers.Components.Video;


using Sturfee.Unity.XR.Core.Utilities;
using System.IO;

public class SampleVideoProvider : VideoProviderBase
{
    private Camera _videoCamera;

    private Canvas _canvas;
    private RawImage _rawImage;
    private AspectRatioFitter _imageFitter;

    private Texture2D _currentFrame;

    private SampleManager _sampleManager;
       
    private void Start()
    {        
        _sampleManager = GetComponent<SampleManager>();

        CreateCamera();
        CreateCanvas();
    }

    private void Update()
    {
        _currentFrame = _sampleManager.GetFrameAtCurrentIndex();
        _rawImage.texture = _currentFrame;
    }

    public override bool IsPortrait()
    {
        return false;
    }

    public override int GetWidth()
    {
        return 1280;
    }

    public override int GetHeight()
    {
        return 720;
    }

    public override Camera GetVideoCamera()
    {
        return _videoCamera;
    }

    public override Texture2D GetCurrentFrame()
    {
        //Debug.Log(" Frame Num : " + _currentFrame.name);
        return _currentFrame;
    }

    public override bool IsSupported()
    {
        return true;
    }

    public override void Destroy()
    {
        if(_videoCamera != null)
        {
            //If session is destryoyed without destroying the scene
            UnityEngine.Object.Destroy(_videoCamera.gameObject);
            UnityEngine.Object.Destroy(_canvas.gameObject);
        }
    }

    private void CreateCamera()
    {
        _videoCamera = new GameObject().AddComponent<Camera>();
        _videoCamera.name = "Sample video provider Background Camera";
        _videoCamera.depth = -100;
        _videoCamera.nearClipPlane = 0.1f;
        _videoCamera.farClipPlane = 2000f;
        _videoCamera.orthographic = true;
        _videoCamera.clearFlags = CameraClearFlags.Color;
        _videoCamera.backgroundColor = Color.black;
        _videoCamera.renderingPath = RenderingPath.Forward;

        // add to proper layer and set culling pTestImageroperties
        _videoCamera.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);
        _videoCamera.cullingMask = 1 << LayerMask.NameToLayer(SturfeeLayers.Background);
    }

    private void CreateCanvas()
    {
        _canvas = new GameObject().AddComponent<Canvas>();
        _canvas.name = "Sample Video Provider Bg Render Canvas";
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;

        _canvas.worldCamera = _videoCamera;
        _canvas.planeDistance = _videoCamera.farClipPlane - 50.0f;
        _canvas.gameObject.layer = LayerMask.NameToLayer(SturfeeLayers.Background);

        _rawImage = new GameObject().AddComponent<RawImage>();
        _rawImage.name = "Raw Image";
        _rawImage.transform.parent = _canvas.transform;
        _rawImage.transform.localPosition = Vector3.zero;
        _rawImage.transform.localScale = Vector3.one;

        _imageFitter = _rawImage.gameObject.AddComponent<AspectRatioFitter>();
        _imageFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

        _imageFitter.aspectRatio = 1280.0f / 720.0f;

    }
       
   
}
