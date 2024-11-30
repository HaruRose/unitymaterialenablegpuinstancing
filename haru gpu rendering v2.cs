using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HaruGPURenderingV2 : EditorWindow
{
    private static bool checkLiltoon = true;
    private static bool checkUnsupported = false;

    [MenuItem("Tools/haru gpu rendering v2", false)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(HaruGPURenderingV2));
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Shaders to Check", EditorStyles.boldLabel);
        checkLiltoon = EditorGUILayout.Toggle("Check Liltoon", checkLiltoon);
        checkUnsupported = EditorGUILayout.Toggle("Check Unsupported Shaders", checkUnsupported);

        if (GUILayout.Button("Apply GPU Instancing"))
        {
            CheckAndEnableGPUInstancing();
        }
    }

    private static void CheckAndEnableGPUInstancing()
    {
        // Get all material assets in the project
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int materialsModifiedCount = 0;

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material == null) continue;

            bool shouldCheck = (checkLiltoon && IsLiltoon(material)) ||
                               (checkUnsupported && IsUnsupported(material));

            if (shouldCheck && !material.enableInstancing)
            {
                // Enable GPU instancing and ensure no other property is changed
                Undo.RecordObject(material, "Enable GPU Instancing");
                material.enableInstancing = true;
                materialsModifiedCount++;

                // Ensure no additional code is added to the material
                if (!ValidateMaterialChanges(material))
                {
                    material.enableInstancing = false;
                    materialsModifiedCount--;
                }
            }
        }

        EditorUtility.DisplayDialog("haru gpu rendering v2",
            materialsModifiedCount > 0
                ? "Enabled GPU instancing for " + materialsModifiedCount + " materials."
                : "No materials needed GPU instancing enabled.", "OK");

        AssetDatabase.SaveAssets();
    }

    private static bool IsLiltoon(Material material)
    {
        return material.shader.name.Contains("lilToon");
    }

    private static bool IsUnsupported(Material material)
    {
        // Treat all shaders that are not "liltoon" as unsupported shaders
        return !IsLiltoon(material);
    }

    private static bool ValidateMaterialChanges(Material material)
    {
        // Ensure that no additional code or unintended changes have been made to the material
        // This is a placeholder function and should include your validation logic as needed
        return material.enableInstancing; // Example check; customize as necessary
    }
}
