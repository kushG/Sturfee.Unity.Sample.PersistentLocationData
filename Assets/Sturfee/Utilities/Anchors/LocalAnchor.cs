using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Constants.Enums;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class LocalAnchor : MonoBehaviour
    {
        public bool IsEnabled = true;
        public Transform Target;
        public GpsPosition GpsPosition;

        private GameObject _helper;

        [SerializeField]
        private bool _maintainRelativeDistance = false;
        [SerializeField]
        private Vector3 _offset = Vector3.zero;

        private void Awake()
        {
            SturfeeEventManager.Instance.OnLocalizationComplete += HandleLocalizationComplete;
            SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;

            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("XRCamera").transform;
            }

            if (Target == null)
            {
                Debug.LogError("LocalAnchor -> Target == null");
            }

            _helper = new GameObject();
            _helper.name = "LocalAnchorHelper";
            _helper.transform.SetParent(Target);
        }

        private void OnEnable()
        {
            if (_helper == null)
            {
                _helper = new GameObject();
                _helper.name = "LocalAnchorHelper";
            }
            _helper.transform.SetParent(Target);
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnLocalizationComplete -= HandleLocalizationComplete;
            SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;
        }

        private void LateUpdate()
        {
            if (_maintainRelativeDistance)
            {
                transform.position = Target.position + _offset;
                _helper.transform.position = transform.position;
                _helper.transform.rotation = transform.rotation;
            }
        }

        private void OnLocalizationLoading()
        {
            if (IsEnabled)
            {
                if (_offset == Vector3.zero)
                {
                    _offset = (transform.position - Target.position);
                }

                _maintainRelativeDistance = true;
            }
        }

        private void HandleLocalizationComplete(AlignmentStatus status)
        {
            if (status == AlignmentStatus.Done)
            {
                _maintainRelativeDistance = false;

                GpsPosition = XRSessionManager.GetSession().LocalPositionToGps(transform.position);
                _offset = Vector3.zero;

                StartCoroutine(DelayedSetFinalPosition());
            }
        }

        private void HandleFakeLocalizationRequest()
        {
            HandleLocalizationComplete(AlignmentStatus.Done);
        }

        private IEnumerator DelayedSetFinalPosition()
        {
            yield return new WaitForEndOfFrame();

            transform.position = _helper.transform.position;
            transform.rotation = _helper.transform.rotation;

            yield return new WaitForEndOfFrame();

            transform.position = _helper.transform.position;
            transform.rotation = _helper.transform.rotation;
        }

        #region Editor UI

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Sturfee/LocalAnchor-Icon.png", true);
        }

        [MenuItem("GameObject/Create Other/Sturfee/LocalAnchor")]
        static void CreateNewWorldAnchor(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("LocalAnchor");
            go.AddComponent<LocalAnchor>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LocalAnchor))]
    [CanEditMultipleObjects]
    public class LocalAnchorEditor : Editor
    {
        private void OnSceneGUI()
        {
            LocalAnchor _target = (LocalAnchor)target;

            Handles.CubeHandleCap(
                0,
                _target.transform.position + new Vector3(0f, -0.5f, 0f),
                _target.transform.rotation * Quaternion.LookRotation(Vector3.up),
                2,
                EventType.Repaint
            );
        }
    }
#endif
}