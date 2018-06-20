using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.Video;
using Sturfee.Unity.XR.Core.Constants;

#if UNITY_ANDROID
using GoogleARCore;
#endif

public class ArCoreVideoProvider :  VideoProviderBase
{

    private Camera _videoCamera;

    private void Awake()
    {
        _videoCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// Start playing the video in background
    /// </summary>
    public override  void Play()
    {

    }

    /// <summary>
    /// Pause the video
    /// </summary>
    public override  void Pause()
    {

    }

    /// <summary>
    /// Stop the video
    /// </summary>
    public override  void Stop()
    {

    }

    /// <summary>
    /// Returns current frame of video as an image
    /// </summary>
    /// <returns></returns>
    public override  Texture2D GetCurrentFrame()
    {
        // Create a temporary render texture 
        RenderTexture tempRT = RenderTexture.GetTemporary(GetWidth(), GetHeight(), 0, RenderTextureFormat.ARGB32);

        #if UNITY_ANDROID
        Graphics.Blit(Texture2D.whiteTexture, tempRT, GetComponent<ARCoreBackgroundRenderer>().BackgroundMaterial);
        #endif


        // Copy the tempRT to a regular texture by reading from the current render target (i.e. tempRT)
        var snap = new Texture2D(GetWidth(), GetHeight());
        snap.ReadPixels(new Rect(0, 0, GetWidth(), GetHeight()), 0, 0, false); // ReadPixels(Rect source, ...) ==> Rectangular region of the view to read from. ***Pixels are read from current render target.***
        snap.Apply();

        return snap;
    }

    /// <summary>
    /// Gets the height of the video texture
    /// </summary>
    /// <returns>The height.</returns>
    public override  int GetHeight()
    {
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            return 720;
        }

        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            return 1280;
        }

        return 720;
    }

    /// <summary>
    /// Gets the width of the video texture
    /// </summary>
    /// <returns>The width.</returns>
    public override  int GetWidth()
    {    
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            return 1280;
        }

        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            return 720;
        }
        return 1280;
    }

    /// <summary>
    /// Determines whether the screen orientation is portrait.
    /// </summary>
    /// <returns><c>true</c> if this instance is portrait; otherwise, <c>false</c>.</returns>
    public override  bool IsPortrait()
    {
        return (Screen.orientation == ScreenOrientation.Portrait);
    }

    /// <summary>
    /// Determines if the provider supports the device it is running on.
    /// </summary>
    /// <returns><c>true</c> if the device is supported; otherwise, <c>false</c>.</returns>
    public override  bool IsSupported()
    {
        return ProviderHelper.ArCoreSupported;
    }     

    public override Camera GetVideoCamera()
    {
        return _videoCamera;
    }

	public override void Destroy ()
	{
		
	}
        
}
