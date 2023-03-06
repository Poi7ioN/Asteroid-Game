#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// declares a new custom editor for the PowerSpawnerConfig class

[CustomEditor(typeof(PowerSpawnerConfig))]
public class PowerSpawnerConfigEditor : Editor 
{
    // serialized properties that will be displayed in the Unity editor

    private SerializedProperty _isPermanentProperty;
    private SerializedProperty _powerActingTimeProperty;
    private SerializedProperty _powerPrefabProperty;
    private SerializedProperty _powerProbabilityProperty;
    private SerializedProperty _powerAliveTimeProperty;
    private SerializedProperty _powerHoverSpeedProperty;

    private void OnEnable() // initialize the serialized properties by finding them by name
    {
        _isPermanentProperty      = serializedObject.FindProperty("_isPermanent");
        _powerActingTimeProperty  = serializedObject.FindProperty("_powerActingTime");
        _powerPrefabProperty      = serializedObject.FindProperty("_powerPrefab");
        _powerProbabilityProperty = serializedObject.FindProperty("_powerProbability");
        _powerAliveTimeProperty   = serializedObject.FindProperty("_powerAliveTime");
        _powerHoverSpeedProperty  = serializedObject.FindProperty("_powerHoverSpeed");
    }

    public override void OnInspectorGUI() // called when the editor needs to display its GUI
    {
        serializedObject.Update();                                            // update the serialized object to reflect any changes made by the user
        EditorGUILayout.PropertyField(_powerPrefabProperty);                 // display the powerPrefab property field in the editor
        EditorGUILayout.PropertyField(_powerProbabilityProperty);           // display the powerProbability property field in the editor
        EditorGUILayout.PropertyField(_powerAliveTimeProperty);            // display the powerAliveTime property field in the editor
        EditorGUILayout.PropertyField(_powerHoverSpeedProperty);          // display the powerHoverSpeed property field in the editor

        EditorGUILayout.PropertyField(_isPermanentProperty);            // display the isPermanent property field in the editor

        // allow to set power acting time property only if the power up is a temporary acting powerup.
        if (!_isPermanentProperty.boolValue)                           
        {
            EditorGUILayout.PropertyField(_powerActingTimeProperty); 
        }

        serializedObject.ApplyModifiedProperties();               // apply any changes made by the user to the serialized object
    }
}
#endif