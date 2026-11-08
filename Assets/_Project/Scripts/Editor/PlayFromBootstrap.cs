using UnityEditor;
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public static class PlayFromBootstrap
{
    const string BootstrapPath =
"Assets/_Project/Scenes/Sandbox/Scn_Ilham/Scn_BootstrapI.unity";
    static PlayFromBootstrap()
    {
        EditorApplication.delayCall += Apply;
    }
    static void Apply()
    {
        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapPath);
    }
}