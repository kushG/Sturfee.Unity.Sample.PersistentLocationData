using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;

public class GridEffect : MonoBehaviour {

    public bool ScaleByDistance = true;
    public bool ApplyFadingEffect;

    private float _fadeTime = 4.0f;

    public void Start()
    {
        ShowGrid();
    }

    public void ShowGrid()
    {
        gameObject.SetActive(true);

        if (ScaleByDistance)
        {
            Vector3 pos = (XRSessionManager.GetSession() != null) ? XRSessionManager.GetSession().GetXRCameraPosition() : Camera.main.transform.position;

            var distance = Vector3.Distance(pos, transform.position);
            var clampedDistance = Mathf.Clamp((distance/100), 4, 10);
            transform.localScale = new Vector3(clampedDistance, clampedDistance, clampedDistance);
        }

        if (ApplyFadingEffect)
        {
            FadeInAndOut(gameObject);
        }
    }

    private void FadeInAndOut(GameObject instance)
    {
        var renderer = instance.GetComponent<Renderer>();
        if(renderer == null)
        {
            renderer = instance.GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            return;
        }

        StartCoroutine(DelayedHide());

        //StartCoroutine(FadeInAsync(renderer, FadeOut));
        StartCoroutine(FadeInAsync(renderer, FadeOut));
    }

    IEnumerator FadeTo(Renderer renderer, float aValue, float aTime, Action<Renderer> callback)
    {
        float alpha = renderer.material.color.a;
        for (float t = 0.0f; t < _fadeTime; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            renderer.material.color = newColor;
            yield return null;
        }

        if (callback != null)
        {
            callback(renderer);
        }       
    }

    private IEnumerator FadeInAsync(Renderer renderer, Action<Renderer> callback)
    {
        return FadeTo(renderer, _fadeTime, 0.3f, callback);
    }

    private void FadeOut(Renderer renderer)
    {
        StartCoroutine(FadeOutAsync(renderer, Hide));
    }

    private IEnumerator FadeOutAsync(Renderer renderer, Action<Renderer> callback)
    {
        //yield return new WaitForSeconds(0.5f);
        //StartCoroutine(FadeTo(renderer, 0.0f, 0.25f, callback));

        return FadeTo(renderer, 0.0f, 0.3f, callback);

        //var alpha = 0.0f;

        //while (alpha > 0)
        //{
        //    alpha += Time.deltaTime;// * fadingOutSpeed;

        //    Color newColor = renderer.material.color;
        //    newColor.a = Mathf.Min(newColor.a, alpha);
        //    newColor.a = Mathf.Clamp(newColor.a, 0.0f, 1.0f);
        //    renderer.material.SetColor("_Color", newColor);

        //    yield return null;
        //}
    }

    private IEnumerator DelayedHide()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);

        //yield return new WaitForSeconds(1);
        //Destroy(_instance.transform.root.gameObject);
        //Destroy(this.transform.root.gameObject);
    }

    private void Hide(Renderer renderer)
    {
        gameObject.SetActive(false);
    }
}
