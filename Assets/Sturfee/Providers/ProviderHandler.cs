using UnityEngine;
using Sturfee.Unity.XR.Providers.Components.ArCamera;
using Sturfee.Unity.XR.Providers.Components.Imu;
using Sturfee.Unity.XR.Providers.Components.Gps;
using Sturfee.Unity.XR.Providers.Components.Video;
using Sturfee.Unity.XR.Providers.Components.Lighting;

[ExecuteInEditMode]
public class ProviderHandler: MonoBehaviour
{
    private SturfeeXRSession _xrSession;

    private string _currentProviderSetName;
    private string _previousProviderSetName;

    private void OnEnable()
    {
        hideFlags = HideFlags.HideInInspector;

        _xrSession = GetComponent<SturfeeXRSession>();

        _currentProviderSetName = _xrSession.ProviderSetName;
        _previousProviderSetName = _currentProviderSetName;
    }

    private void Update()
    {
        //Check for Provider set schange
        _currentProviderSetName = _xrSession.ProviderSetName;
        if (_currentProviderSetName != _previousProviderSetName)
        {
            OnProviderSetChange();
            _previousProviderSetName = _currentProviderSetName;
        }
    }
           
    private void OnProviderSetChange()
    {
        Clear();

        if (_currentProviderSetName == "Add New")               
        {
            return;
        }

        _xrSession.ProviderSet = (Resources.Load("Provider Sets/" + _xrSession.ProviderSetName) as GameObject).GetComponent<ProviderSet>();
//
        CopyProviderComponents(_xrSession.ProviderSet.gameObject, _xrSession.gameObject);

        if (_xrSession.ProviderSet.ImuProvider != null)
        {
            _xrSession.ImuProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.ImuProvider.GetType()) as ImuProviderBase;
        }
        if (_xrSession.ProviderSet.GpsProvider != null)
        {
            _xrSession.GpsProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.GpsProvider.GetType()) as GpsProviderBase;
        }
        if (_xrSession.ProviderSet.VideoProvider != null)
        {
            _xrSession.VideoProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.VideoProvider.GetType()) as VideoProviderBase;
        }
        if (_xrSession.ProviderSet.CameraProvider != null)
        {
            _xrSession.CameraProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.CameraProvider.GetType()) as CameraProviderBase;
        }
        if (_xrSession.ProviderSet.LightingProvider != null)
        {
            _xrSession.LightingProvider = _xrSession.gameObject.GetComponent(_xrSession.ProviderSet.LightingProvider.GetType()) as LightingProviderBase;
        }
    }

	private void Clear()
    {
        _xrSession.ImuProvider = null;
        _xrSession.GpsProvider = null;
        _xrSession.VideoProvider = null;
        _xrSession.CameraProvider = null;
        _xrSession.LightingProvider = null;

        //First remove all the components that has "RequireComponent" attribute
        foreach(Component co in _xrSession.gameObject.GetComponents<Component>())
        {
            if(co.GetType().IsDefined(typeof(RequireComponent), false) && (co as Camera == null) && (co as SturfeeXRSession == null))
            {
                //TODO: If a component has RequireComponent and is also a part of another component's RequireComponent
                DestroyImmediate(co);
            }
        }

        foreach(Component co in _xrSession.gameObject.GetComponents<Component>())
        {
            var providersComponent = co as SturfeeXRSession;
            var transformComponent = co as Transform;
            var providerHandler = co as ProviderHandler;
            if(providersComponent == null && transformComponent == null && providerHandler == null)
            {
                DestroyImmediate(co);
            }
        }
    }

    private void CopyProviderComponents(GameObject sourceGO, GameObject targetGO)
    {
        foreach (var component in sourceGO.GetComponents<Component>())
        {
            var componentType = component.GetType();
            if (componentType != typeof(Transform) &&
                componentType != typeof(ProviderSet)
            )
            {
                #if UNITY_EDITOR
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGO);
                #endif
            }
        }
    }
}
