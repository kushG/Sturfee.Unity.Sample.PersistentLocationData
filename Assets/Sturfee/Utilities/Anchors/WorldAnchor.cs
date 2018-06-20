using UnityEngine;
using Sturfee.Unity.XR.Core.Models.Location;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using System;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Constants.Enums;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class WorldAnchor : MonoBehaviour
    {
        public enum SetPositionOn
        {
            SessionReady,
            LocalizationComplete
        }

        [Tooltip("When should the GPS location be used: after the XR Session is ready or after Localization")]
        public SetPositionOn SetPositionWhen = SetPositionOn.SessionReady;
        public GpsPosition GpsPosition;

        [Tooltip("Update position in the update loop (true) or only when the XRSession is ready (false)")]
        public bool Dynamic = false;

        [Tooltip("Update GPS location of this object from its local position (when set to true)")]
        [HideInInspector]
        public bool OverrideWithLocal = false;

        [HideInInspector]
        public string _id;// = Guid.NewGuid().ToString();

        private void Awake()
        {
            if (SetPositionWhen == SetPositionOn.SessionReady)
            {
                SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
            }
            else
            {
                SturfeeEventManager.Instance.OnLocalizationComplete += HandleLocalizationComplete;
            }
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
            SturfeeEventManager.Instance.OnLocalizationComplete -= HandleLocalizationComplete;
        }

        private void LateUpdate()
        {
            if (Dynamic)
            {
                transform.position = XRSessionManager.GetSession().GpsToLocalPosition(GpsPosition);
            }
            else if (OverrideWithLocal)
            {
                GpsPosition = XRSessionManager.GetSession().LocalPositionToGps(transform.position);
            }
        }

        private void OnSessionReady()
        {
            transform.position = XRSessionManager.GetSession().GpsToLocalPosition(GpsPosition);
        }

        private void HandleLocalizationComplete(AlignmentStatus status)
        {
            if (status == AlignmentStatus.Done)
            {
                transform.position = XRSessionManager.GetSession().GpsToLocalPosition(GpsPosition);
            }
        }

        #region Editor UI

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Sturfee/WorldAnchor-Icon.png", true);
        }

        [MenuItem("GameObject/Create Other/Sturfee/WorldAnchor")]
        static void CreateNewWorldAnchor(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("WorldAnchor");
            go.AddComponent<WorldAnchor>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif

        #endregion
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(WorldAnchor))]
    [CanEditMultipleObjects]
    public class WorldAnchorEditor : Editor
    {
        private static bool _inPlayMode = false;
        private static Dictionary<string, WorldAnchor> _targets = new Dictionary<string, WorldAnchor>();
        private static Dictionary<string, WorldAnchor> _allWorldAnchors = new Dictionary<string, WorldAnchor>();
        private static bool _loading = false;


        //Called when the object is first added to scene
        private void Reset()
        {
            var _target = (WorldAnchor)target;
            if (string.IsNullOrEmpty(_target._id) || EditorPrefs.HasKey(_target._id))
            {
                if (_target.GetInstanceID() == EditorPrefs.GetInt(_target._id))
                {
                    //Debug.Log("GUID already created...");
                }
                else
                {
                    //Debug.Log("Creating Guid");
                    _target._id = Guid.NewGuid().ToString();
                    EditorPrefs.SetInt(_target._id, _target.GetInstanceID());
                }
            }
            else
            {
                Debug.LogWarning("GUID not created for one of the WorldAnchor");
            }

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            _inPlayMode = Application.isPlaying;
            EditorApplication.playModeStateChanged += CheckPlayMode;

            var _target = (WorldAnchor)target;
            if (string.IsNullOrEmpty(_target._id))
            {
                _target._id = Guid.NewGuid().ToString();
            }

            if (_inPlayMode)
            {
                if (GUILayout.Button("Edit"))
                {
                    _target.OverrideWithLocal = true;
                }

                if (_target.OverrideWithLocal)
                {
                    if (GUILayout.Button("Save"))
                    {
                        if (_targets == null)
                        {
                            _targets = new Dictionary<string, WorldAnchor>();
                        }

                        _targets.Add(_target._id, _target);

                        EditorApplication.playModeStateChanged += OnChangePlayModeState;

                        _target.OverrideWithLocal = false;
                    }
                }
            }
        }

        private void OnSceneGUI()
        {
            WorldAnchor _target = (WorldAnchor)target;

            Handles.CircleHandleCap(
                0,
                _target.transform.position + new Vector3(0f, -0.5f, 0f),
                _target.transform.rotation * Quaternion.LookRotation(Vector3.up),
                2,
                EventType.Repaint
            );
        }

        private void CheckPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _inPlayMode = true;
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                _inPlayMode = false;
            }
        }

        void OnChangePlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                WorldAnchor _target = (WorldAnchor)target;
                string saveKey = "WorldAnchorSaveData_" + _target._id;

                string stringSerializedSelection = EditorJsonUtility.ToJson(_target.GpsPosition);
                EditorPrefs.SetString(saveKey, stringSerializedSelection);
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (_loading)
                {
                    return;
                }
                _loading = true;

                EditorUtility.DisplayProgressBar("Save Edit Mode Changes", "Restoring Edit Mode GameObjects...", 0);

                var allWorldAnchors = FindObjectsOfType<WorldAnchor>();
                foreach (var worldAnchor in allWorldAnchors)
                {
                    if (_targets.ContainsKey(worldAnchor._id))
                    {
                        if (_targets[worldAnchor._id] == null)
                        {
                            _targets[worldAnchor._id] = worldAnchor;
                        }
                    }
                }

                foreach (var id in _targets.Keys)
                {
                    string saveKey = "WorldAnchorSaveData_" + id;

                    if (!EditorPrefs.HasKey(saveKey))
                    {
                        continue;
                    }

                    string serializedDataString = EditorPrefs.GetString(saveKey);
                    EditorPrefs.DeleteKey(saveKey);

                    try
                    {
                        var updatedGps = new GpsPosition();
                        EditorJsonUtility.FromJsonOverwrite(serializedDataString, updatedGps);

                        var target = _targets[id];
                        target.GpsPosition = updatedGps;
                    }
                    catch
                    {
                        Debug.LogError("Error restoring changes to WorldAnchor during PlayMode");
                    }
                }
                EditorUtility.ClearProgressBar();
                _targets.Clear();
                _loading = false;
            }
        }
    }
#endif
}