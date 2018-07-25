using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeTarget : MonoBehaviour {

	[SerializeField]
	private GameObject _alignmentRing;
	[SerializeField]
	private GameObject _confirmationCircle;
	[SerializeField]
	private GameObject _checkmark;

	private Image _confirmationCircleImage;
	private Color _confirmationCircleColor;

	// Use this for initialization
	void Start () {
		_confirmationCircleImage = _confirmationCircle.GetComponent<Image> ();
		_confirmationCircleColor = _confirmationCircleImage.color;
	}

	public void TargetAlignedSuccess()
	{
		GetComponent<Collider> ().enabled = false;
		_alignmentRing.SetActive (false);
		_confirmationCircle.SetActive (true);
		_checkmark.SetActive (true);

		Fade ();
		//		Invoke ("Fade", 0.2f);
		//		StartCoroutine (Fade ());
	}

	private void Fade()
	{
		//		yield return new WaitForFixedUpdate ();

		_confirmationCircleColor.a -= 0.02f;
		_confirmationCircleImage.color = _confirmationCircleColor;

		if (_confirmationCircleColor.a > 0)
		{
			Invoke ("Fade", 0.02f);
			//			StartCoroutine (Fade ());
		}
		else
		{
			Destroy (this.gameObject);
		}
	}
}
