using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Core.Constants;
using UnityEngine.XR.iOS;
using Sturfee.Unity.XR.Providers.Components.Video;

[RequireComponent(typeof(Camera))]
public class ArKitVideoProvider : VideoProviderBase
{
    public Material ClearMaterial;    

    private Camera _arKitBackgroundCamera;

    private CommandBuffer _videoCommandBuffer;
    private Texture2D _videoTextureY;
    private Texture2D _videoTextureCbCr;
    private Matrix4x4 _displayTransform; 
    private Resolution _arKitNativeResolution;

    private bool _bCommandBufferInitialized;

    private WebCamTexture _webCamtexture;
    private bool _unityWebCamInUse;

    private GCHandle _yHandle;
    private GCHandle _uvHandle;
    private int currentFrameIndex;
    private byte[] m_textureYBytes;
    private byte[] m_textureUVBytes;
    private byte[] m_textureYBytes2;
    private byte[] m_textureUVBytes2;

    private UnityARCamera _camera;
    private UnityARSessionNativeInterface _session;  


    private void Start()
    {
        
        _arKitBackgroundCamera = GetComponent<Camera>();

        if (_arKitBackgroundCamera == null)
        {
            Debug.LogError("ArKit's Background/Video camera is not available");
        }

        UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateFrame;
        _bCommandBufferInitialized = false;

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

        // apply video provider camera's material to 'whiteTexture' and write it out to tempRT
        // IOS --> UnityARVIdeo
        //Android --> Tango's Background renderer
        Graphics.Blit(Texture2D.whiteTexture, tempRT, ClearMaterial);

        // Copy the tempRT to a regular texture by reading from the current render target (i.e. tempRT)
        var snap = new Texture2D(GetWidth(), GetHeight());
        snap.ReadPixels(new Rect(0, 0, GetWidth(), GetHeight()), 0, 0, false); // ReadPixels(Rect source, ...) ==> Rectangular region of the view to read from. ***Pixels are read from current render target.***
        snap.Apply();           

        return snap;
    
    }

    /// <summary>
    /// Gets the height od the video texture
    /// </summary>
    /// <returns>The height.</returns>
    public override  int GetHeight()
    {
        return 720;
        return _arKitNativeResolution.height;
    }

    /// <summary>
    /// Gets the width of the video texture
    /// </summary>
    /// <returns>The width.</returns>
    public override  int GetWidth()
    {
        return 1280;
        return _arKitNativeResolution.width;        
    }

    /// <summary>
    /// Determines whether the screen orientation is portrait.
    /// </summary>
    /// <returns><c>true</c> if this instance is portrait; otherwise, <c>false</c>.</returns>
    public override  bool IsPortrait()
    {
        //TODO: Update me once we start supporting Portrait mode
        return Screen.orientation == ScreenOrientation.Portrait;
    }

    /// <summary>
    /// Determines if the provider supports the device it is running on.
    /// </summary>
    /// <returns><c>true</c> if the device is supported; otherwise, <c>false</c>.</returns>
    public override  bool IsSupported()
    {
        return ProviderHelper.ArKitSupported;
    }        

    public override Camera GetVideoCamera()
    {
        return _arKitBackgroundCamera;
    }

	public override void Destroy ()
	{
		
	}
        
    private void UpdateFrame(UnityARCamera cam)
    {
        if (_unityWebCamInUse)
        {
            return;
        }
			      
        _displayTransform = new Matrix4x4();
        _displayTransform.SetColumn(0, cam.displayTransform.column0);
        _displayTransform.SetColumn(1, cam.displayTransform.column1);
        _displayTransform.SetColumn(2, cam.displayTransform.column2);
        _displayTransform.SetColumn(3, cam.displayTransform.column3);       
    }

    private void UseWebCam()
    {
        _unityWebCamInUse = true;

        //set up camera
        WebCamDevice[] devices = WebCamTexture.devices;
        string backCamName = "";
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Device:" + devices[i].name + "IS FRONT FACING:" + devices[i].isFrontFacing);

            if (!devices[i].isFrontFacing)
            {
                backCamName = devices[i].name;
            }
        }

        _webCamtexture = new WebCamTexture(backCamName, 1280, 720, 24);
        _webCamtexture.Play();
    }

    private IEnumerator StopUnityWebCame()
    {
        yield return new WaitForEndOfFrame();

        _webCamtexture.Stop();
        _unityWebCamInUse = false;
    }

    void InitializeCommandBuffer()
    {		
        _videoCommandBuffer = new CommandBuffer(); 
        _videoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, ClearMaterial);
        _arKitBackgroundCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _videoCommandBuffer);
        _bCommandBufferInitialized = true;

    }

    void OnDestroy()
    {		
        _arKitBackgroundCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _videoCommandBuffer);
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateFrame;
        _bCommandBufferInitialized = false;
    }

    #if !UNITY_EDITOR

    public void OnPreRender()
    {
        if (_unityWebCamInUse)
        {
            return;
        }

        ARTextureHandles handles = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetARVideoTextureHandles();
        if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
        {
            return;
        }

        if (!_bCommandBufferInitialized)
        {
            InitializeCommandBuffer ();
        }

	       
        //TODO: This resolution should be same as Native ArKit texture resolution(1280 * 720).
        //We will set the GetWidth() and getHeight() to this resolution
        _arKitNativeResolution = Screen.currentResolution;

        //HACK
        if (Screen.orientation == ScreenOrientation.Landscape)
        {
            _arKitNativeResolution.width = 1280;
            _arKitNativeResolution.height = 720;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            _arKitNativeResolution.width = 720;
            _arKitNativeResolution.height = 1280;
        }

        // Texture Y
        if (_videoTextureY == null) 
        {
            _videoTextureY = Texture2D.CreateExternalTexture(_arKitNativeResolution.width, _arKitNativeResolution.height,
            TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            ClearMaterial.SetTexture("_textureY", _videoTextureY);
        }

        // Texture CbCr
        if (_videoTextureCbCr == null)
        {
            _videoTextureCbCr = Texture2D.CreateExternalTexture(_arKitNativeResolution.width, _arKitNativeResolution.height,
            TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
            _videoTextureCbCr.filterMode = FilterMode.Bilinear;
            _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
            ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
        }

        _videoTextureY.UpdateExternalTexture(handles.textureY);
        _videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);

        ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
    }

    public void SetYTexure(Texture2D YTex)
    {     
        _videoTextureY = YTex;
    }

    public void SetUVTexure(Texture2D UVTex)
    {
        _videoTextureCbCr = UVTex;
    }

    #else

    public void OnPreRender()
    {
        
        if (!_bCommandBufferInitialized) {
            InitializeCommandBuffer ();
        }

        ClearMaterial.SetTexture("_textureY", _videoTextureY);
        ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);

        ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
    }

    #endif





    IntPtr PinByteArray(ref GCHandle handle, byte[] array)
    {
        handle.Free ();
        handle = GCHandle.Alloc (array, GCHandleType.Pinned);
        return handle.AddrOfPinnedObject ();
    }

    byte [] ByteArrayForFrame(int frame,  byte[] array0,  byte[] array1)
    {
        return frame == 1 ? array1 : array0;
    }

    byte [] YByteArrayForFrame(int frame)
    {
        return ByteArrayForFrame (frame, m_textureYBytes, m_textureYBytes2);
    }

    byte [] UVByteArrayForFrame(int frame)
    {
        return ByteArrayForFrame (frame, m_textureUVBytes, m_textureUVBytes2);
    }


}
