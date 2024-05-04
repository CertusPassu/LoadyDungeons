using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[CustomEditor(typeof(PlayerConfigurator))]
public class PlayerConfiguratorEditor : Editor
{
    SerializedProperty addressProp;
    private static List<string> addressOptions = new List<string>();

    private void OnEnable()
    {
        addressProp = serializedObject.FindProperty("m_Address");
        FetchAddressableKeys();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        if (addressOptions.Count > 0)
        {
            int currentIndex = GetCurrentIndex(addressProp.stringValue);
            int newIndex = EditorGUILayout.Popup("Address", currentIndex, addressOptions.ToArray());

            if (newIndex != currentIndex)
            {
                addressProp.stringValue = addressOptions[newIndex];
                serializedObject.ApplyModifiedProperties();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Addressable keys found.");
        }
    }

    private void FetchAddressableKeys()
    {
        addressOptions.Clear();

        // Path to the Addressables asset data (JSON format)
        string path = "Assets/AddressableAssetsData/AssetGroups";
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(file);
                if (asset != null)
                {
                    // This will be a very crude way of extracting keys; you might need a JSON parser here
                    string contents = asset.text;
                    var matches = System.Text.RegularExpressions.Regex.Matches(contents, "\"assetName\": \"([^\"]+)\"");
                    foreach (var match in matches.Cast<System.Text.RegularExpressions.Match>())
                    {
                        string key = match.Groups[1].Value;
                        if (!string.IsNullOrEmpty(key))
                            addressOptions.Add(key);
                    }
                }
            }
        }

        addressOptions = addressOptions.Distinct().ToList();
        addressOptions.Sort();
    }

    private int GetCurrentIndex(string currentAddress)
    {
        int index = addressOptions.IndexOf(currentAddress);
        return index >= 0 ? index : 0;
    }
}
