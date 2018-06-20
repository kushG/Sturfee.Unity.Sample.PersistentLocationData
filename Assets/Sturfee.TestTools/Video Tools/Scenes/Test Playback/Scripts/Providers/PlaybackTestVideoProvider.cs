using UnityEngine;
using UnityEngine.Video;
using Sturfee.Unity.XR.Providers.Components.Video;
using System.IO;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class PlaybackTestVideoProvider : VideoProviderBase 
    {
        [Header("Configuration")]
        public VideoPlayer VideoPlayer;

        private VideoClip _videoClip;
        private Texture2D _videoFrame;

        private void Awake()
        {
            if (VideoPlayer == null)
            {
                VideoPlayer = GetComponent<VideoPlayer>();
            }
        }

        private void Start()
        {
            TestPlaybackManager.Instance.OnVideoReady += OnVideoReady;

            if (TestPlaybackManager.Instance.LocalizeWhilePlaying)
            {
                _videoFrame = new Texture2D(2, 2);
            }
        }
                  
        public override int GetHeight()
        {
            return (int)_videoClip.height;
        }

        public override int GetWidth()
        {
            return (int)_videoClip.width;
        }

        public override Texture2D GetCurrentFrame()
        {
            if (TestPlaybackManager.Instance.LocalizeWhilePlaying)
            {
                // UNCOMMENT THIS TO SEE THE IMAGE SENT TO THE VPS
                //// Encode texture into PNG
                //byte[] bytes = _videoFrame.EncodeToPNG();
                //// For testing purposes, also write to a file in the project folder
                //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

                return _videoFrame;
            }
            else
            {
                return null;
            }
        }

        public override Camera GetVideoCamera()
        {
            return GetComponent<Camera>();
        }

        public override bool IsPortrait()
        {
            return false;
            //return base.IsPortrait();
        }

        public override bool IsSupported()
        {
            return true;
        }

		public override void Destroy ()
		{
			
		}
    	
        public void Play()
        {
            VideoPlayer.Play();
        }

        public void PausePlayback()
        {
            VideoPlayer.Pause();
        }

        private void OnVideoReady (VideoClip videoClip)
        {   
            VideoPlayer = gameObject.AddComponent<VideoPlayer>();
            VideoPlayer.sendFrameReadyEvents = true;

            if (TestPlaybackManager.Instance.LocalizeWhilePlaying)
            {
                VideoPlayer.frameReady += OnNewFrame;
            }


            VideoPlayer.playOnAwake = false;
            VideoPlayer.waitForFirstFrame = false;
            VideoPlayer.isLooping = TestPlaybackManager.Instance.IsLooping;
            VideoPlayer.clip = videoClip;

            _videoClip = videoClip;

        }

        private void OnNewFrame(VideoPlayer source, long frameIdx)
        {
            RenderTexture renderTexture = source.texture as RenderTexture;

            if (_videoFrame.width != renderTexture.width || _videoFrame.height != renderTexture.height)
            {
                _videoFrame.Resize(renderTexture.width, renderTexture.height);
            }
            RenderTexture.active = renderTexture;
            _videoFrame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            _videoFrame.Apply();
            RenderTexture.active = null;
        }
    }
}