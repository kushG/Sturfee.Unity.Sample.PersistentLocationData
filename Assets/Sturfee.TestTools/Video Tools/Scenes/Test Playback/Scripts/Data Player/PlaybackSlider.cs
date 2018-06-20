using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sturfee.Unity.XR.Package.TestTools;
using UnityEngine.UI;

public class PlaybackSlider : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        TestPlaybackManager.Instance.OnPlaybackSliderValueChanged();
    }
}
