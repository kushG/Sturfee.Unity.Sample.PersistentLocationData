using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class VideoController : MonoBehaviour
    {

        [SerializeField]
        private GameObject _play;
        [SerializeField]
        private GameObject _pause;
        [SerializeField]
        private Slider _playbackSlider;

        private int _sessionDataLength;


        private void Update()
        {
            if (!TestPlaybackManager.Instance.IsLooping)
            {
                if(_playbackSlider.value == (_playbackSlider.maxValue - 1))
                {
                    _play.SetActive(true);
                    _pause.SetActive(false);
                }
            }

            if(TestPlaybackManager.Instance.IsPlaying)
            {
                _play.SetActive(false);
                _pause.SetActive(true);
            }
        }
    }
}
