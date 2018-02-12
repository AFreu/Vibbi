using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;

public class ImportPostProcessor : AssetPostprocessor {
    [DllImport("deform_plugin")] private static extern bool InitDeformPlugin(string path);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        InitDeformPlugin(Application.dataPath + "/Plugins/deform_config.xml");
        Debug.Log("Initialized Deform Plugin");
    }
}
