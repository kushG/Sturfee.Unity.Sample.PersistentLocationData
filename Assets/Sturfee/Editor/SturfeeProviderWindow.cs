using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SturfeeXRSession))]
public class SturfeeProviderWindow : Editor
{
    public string[] ProviderSet;
    private int _choiceIndex = 0;   
    private SerializedProperty _provType;
    private SerializedProperty _provString;

    private bool _isShowingConfigWindow;

    private void OnEnable()
    {
        //Default
        Object[] defaultSets = Resources.LoadAll("Provider Sets/Default");
        Object[] customSets = Resources.LoadAll("Provider Sets/Custom");
        Object[] testToolSets = Resources.LoadAll("Provider Sets/Test Tools");

        ProviderSet = new string[defaultSets.Length + customSets.Length + testToolSets.Length + 1];

        for (int i = 0; i < defaultSets.Length + customSets.Length + testToolSets.Length; i++)
        {
            if (i < defaultSets.Length)
            {
                ProviderSet[i] = "Default/" + defaultSets[i].name;
            }
            else if(i < defaultSets.Length + customSets.Length)
            {
                ProviderSet[i] = "Custom/" + customSets[i - defaultSets.Length].name;
            }
            else
            {
                ProviderSet[i] = "Test Tools/" + testToolSets[i - defaultSets.Length - customSets.Length].name;
            }
        }
            
        ProviderSet[ProviderSet.Length - 1] = "Add New";
            
        _provType = serializedObject.FindProperty("_provTypeInt");
        _provString = serializedObject.FindProperty("ProviderSetName");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        serializedObject.Update();

        SturfeeXRSession xrSession = target as SturfeeXRSession;

        _provType.intValue = EditorGUILayout.Popup("Provider Set", _provType.intValue, ProviderSet, GUILayout.Height(20));
        _provString.stringValue = ProviderSet[_provType.intValue];

        if (_provString.stringValue == "Add New")
        {
            SturfeeConfigurationWindow.OpenToSection = 1;
            SturfeeConfigurationWindow.ShowWindow();
            _provType.intValue = 0;
        }

        serializedObject.ApplyModifiedProperties(); 
    }
}
