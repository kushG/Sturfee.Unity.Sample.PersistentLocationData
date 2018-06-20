using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using Sturfee.Unity.XR.Core.Utilities;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;
using UnityEngine.UI;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class TestPlaybackManager : MonoBehaviour
    {
        [Header("UI")]
        public Button LocalizeButton;
        [SerializeField]
        private GameObject _videoController;
        [SerializeField]
        private Slider _playbackSlider;

        [Header("Configuration")]
        public TextAsset PoseDataAsset;
        public VideoClip VideoClip;
        public bool IsLooping;

        public bool LocalizeWhilePlaying;

        public delegate void OnDataReadyEvent(Data[] sessionData);
        public event OnDataReadyEvent OnDataReady;

        public delegate void OnVideoReadyEvent(VideoClip videoClip);
        public event OnVideoReadyEvent OnVideoReady;


        public int DataIndex
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }

        private string _poseDataJson;
        private Data[] _sessionData;

        private float _playbackTime;
        private float _playbackRate;
        private bool _isPlaying;
        public bool IsPlaying { get { return _isPlaying; } }
        private int _count;
        private int _addFactor = 1;
        private VideoPlayer _videoPlayer;

        public bool LocalizationDone { get { return _localizationDone; } }
        private bool _localizationDone = false;

        public static TestPlaybackManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private static TestPlaybackManager _instance;

        private void Awake()
        {

            Application.targetFrameRate = 60;
            if (_instance != null)
            {
                Destroy(_instance);
            }
            _instance = this;

            DontDestroyOnLoad(gameObject);

            SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;

            _poseDataJson = PoseDataAsset.text;
            _sessionData = JsonHelper.FromJson<Data>(_poseDataJson);
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;                    
        }

        private IEnumerator Start()
        {
            Time.fixedDeltaTime = 1.0f / 30.0f;

            yield return new WaitForEndOfFrame();

            OnDataReady.Invoke(_sessionData);
            OnVideoReady.Invoke(VideoClip);

            if (LocalizeWhilePlaying)
            {
                LocalizeButton.gameObject.SetActive(true);
            }
            else
            {
                LocalizeButton.gameObject.SetActive(false);
            }

            int prerenderCount = 0;
            int regularCount = 0;

            StartCoroutine(StartXRSession());
        }


        //private void OnPreRender()
        //{
        //    if (_videoPlayer != null && _videoPlayer.isPlaying)
        //    {
        //        if (_count < _sessionData.Length && _sessionData[_count].PreRenderData)
        //        {
        //            _count++;
        //        }
        //    }
        //}

        //private void Update()
        //{
        //    if (_videoPlayer != null && _videoPlayer.isPlaying)
        //    {
        //        if (_count < _sessionData.Length && !_sessionData[_count].PreRenderData)
        //        {
        //            _count++;
        //        }
        //    }
        //    if (_count >= _sessionData.Length)
        //    {
        //        if (IsLooping)
        //        {
        //            _count = 0;
        //        }
        //        else
        //        {
        //            _count = _sessionData.Length - 1;
        //            _isPlaying = false;
        //            return;
        //        }
        //    }
        //    _playbackSlider.value = (float)_videoPlayer.frame;     
        //}

        private bool _sessionIsReady;

        private void Update()
        {
            if (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                //Get the data(count) corresponding to current video frame
                _count = Mathf.FloorToInt(((float)_videoPlayer.frame / (float)_videoPlayer.frameCount) * _sessionData.Length);

                //Debug.Log("Frame count: " + _videoPlayer.frameCount + " Data Count : " + _sessionData.Length );
                //Debug.Log("Count : " + _count + " \tFrame Num : " + _videoPlayer.frame);   

                if (_count >= _sessionData.Length)
                {
                    if (IsLooping)
                    {
                        _count = 0;
                    }
                    else
                    {
                        _count = _sessionData.Length - 1;
                        _isPlaying = false;
                        return;
                    }
                }

                _playbackSlider.value = (float)_videoPlayer.frame;                
            }

        }
              
        public void PlayPause()
        {
            _isPlaying = !_isPlaying;

            if (_isPlaying)
            {
                FindObjectOfType<PlaybackTestVideoProvider>().Play();
                FindObjectOfType<PlaybackEventManager>().Play();

            }
            else
            {
                FindObjectOfType<PlaybackTestVideoProvider>().PausePlayback();
                FindObjectOfType<PlaybackEventManager>().PausePlayback();

            }
        }

        public void SetLocalizationState(bool finished)
        {
            if (!LocalizeWhilePlaying)
            {
                _localizationDone = finished;
                if (finished)
                {
                    var correctedGps = _sessionData[_count].aiGpsLocation;
                    if (correctedGps.Latitude != 0 && correctedGps.Longitude != 0)
                    {
                        XRSessionManager.GetSession().ForceLocationUpdate(_sessionData[_count].aiGpsLocation);
                    }
                    else
                    {
                        //XRSessionManager.GetSession().ForceLocationUpdate(_sessionData[_count].gpsLocation);
                    }
                }
            }                
        }

        public void OnPlaybackSliderValueChanged()
        {
            _videoPlayer.frame = (long) _playbackSlider.value;
            _count = Mathf.FloorToInt(((float)_videoPlayer.frame / (float)_videoPlayer.frameCount) * _sessionData.Length);
        }

        private IEnumerator StartXRSession()
        {
            yield return new WaitForEndOfFrame();

            FindObjectOfType<SturfeeXRSession>().StartSession();
        }

		private void OnSessionReady ()
		{
			_videoPlayer = FindObjectOfType<SturfeeXRSession> ().GetComponent<VideoPlayer>();
            _videoController.SetActive (true);

            StartCoroutine(SetupPlaybackSlider());

            _sessionIsReady = true;
		}

        private IEnumerator SetupPlaybackSlider()
        {
            while (_videoPlayer == null || _videoPlayer.frameCount == 0)
            {
                yield return null;
            }

            _playbackSlider.minValue = 0;
            _playbackSlider.maxValue = (float)_videoPlayer.frameCount;
            _playbackSlider.wholeNumbers = true;



            //Debug.Log(" FRAME COUNT : " + _videoPlayer.frameCount);
            //Debug.Log(" DATA COUNT : " + _sessionData.Length);
            //Debug.Log(" FRAME RATE : " + _videoPlayer.frameRate);
        }

    }
}
