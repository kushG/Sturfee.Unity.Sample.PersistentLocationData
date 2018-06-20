using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class ToastManager : MonoBehaviour {

        private Canvas _displayCanvas;

        [SerializeField]
        private Canvas _landscapeCanvas;
        [SerializeField]
        private Canvas _portraitCanvas;

        public static ToastManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private static ToastManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = this;
//    		DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            //Uncomment this once Portrait mode is ready
//			if (ScreenOrientationHelper.Instance.OrientationChanged)
//            {
//                UpdateOrientation();
//            }

            _displayCanvas = _landscapeCanvas;

        }

        private void UpdateOrientation()
        {
            if (Screen.orientation == ScreenOrientation.Landscape)
            {
                _displayCanvas = _landscapeCanvas;
            }
            else if (Screen.orientation == ScreenOrientation.Portrait)
            {
                _displayCanvas = _portraitCanvas;
            }
        }

        public void ShowToastTimed(string toastMessage, float durationInSeconds = 2.0f)
        {
            _displayCanvas.gameObject.SetActive(true);
            _displayCanvas.gameObject.GetComponentInChildren<Text>().text = toastMessage;
            StartCoroutine(HideToast(durationInSeconds));
        }

		public void ShowToast(string toastMessage)
		{
			_displayCanvas.gameObject.SetActive(true);
			_displayCanvas.gameObject.GetComponentInChildren<Text>().text = toastMessage;
		}
//
        private IEnumerator HideToast(float duration)
        {
            yield return new WaitForSeconds(duration);

            _displayCanvas.gameObject.SetActive(false);
            _displayCanvas.gameObject.GetComponentInChildren<Text>().text = "";
        }
    }
}