using UnityEngine;
using Sturfee.Unity.XR.Package.Utilities;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class PlaybackEventManager : MonoBehaviour 
    {
        private bool _localizationComplete;
        private bool _localizationFailed;
        private bool _waitingForLocalization;

        private Data[] _sessionData;
        private bool _isPlaying;

        private void Start()
        {
            TestPlaybackManager.Instance.OnDataReady += OnDataReady;
        }

        private void Update()
        {
            if (!_isPlaying || TestPlaybackManager.Instance.LocalizeWhilePlaying)
            {
                return;
            }

            if (!_waitingForLocalization)
            {
                if (_sessionData[TestPlaybackManager.Instance.DataIndex].sessionEvents.alignmentStarted)
                {
                    OnPerformLocalizationRequested();
                    _waitingForLocalization = true;
                }
            }

            if (!_localizationComplete)
            {
                if (_sessionData[TestPlaybackManager.Instance.DataIndex].sessionEvents.alignmentFinished)
                {
                    OnLocalizationComplete();
                    _localizationComplete = true;
                }
            }

            if (!_localizationFailed)
            {
                if (_sessionData[TestPlaybackManager.Instance.DataIndex].sessionEvents.alignmentFailed)
                {
                    OnLocalizationFailed();
                    _localizationFailed = true;
                }
            }

            if (!_waitingForLocalization)
            {
                if (_sessionData[TestPlaybackManager.Instance.DataIndex].sessionEvents.alignmentStarted)
                {
                    OnPerformLocalizationRequested();
                    _waitingForLocalization = true;
                }
            }

            if (TestPlaybackManager.Instance.IsLooping && TestPlaybackManager.Instance.DataIndex == 0)
            {
                _localizationComplete = false;
                _waitingForLocalization = false;
                _localizationFailed = false;
            }
        }

        public void Play()
        {
            _isPlaying = true;
        }

        public void PausePlayback()
        {
            _isPlaying = false;
        }

        private void OnDataReady (Data[] sessionData)
        {
            _sessionData = sessionData;
        }

        private void OnPerformLocalizationRequested()
        {
            ToastManager.Instance.ShowToastTimed("Localization requested...");
            TestPlaybackManager.Instance.SetLocalizationState(false);
        }

        private void OnLocalizationComplete()
        {
            ToastManager.Instance.ShowToastTimed("Localization Complete");
            TestPlaybackManager.Instance.SetLocalizationState(true);
        }

        private void OnLocalizationFailed()
        {
            ToastManager.Instance.ShowToastTimed("Localization failed");
            TestPlaybackManager.Instance.SetLocalizationState(false);
        }
    }

}