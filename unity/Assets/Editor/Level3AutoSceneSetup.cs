using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Level3AutoSceneSetup
{
    private const string BuildGuardKey = "Level3AutoSceneSetup.BuildGuard";

    static Level3AutoSceneSetup()
    {
        EditorApplication.delayCall += TryEnsureLevel3Rooms;
        EditorSceneManager.sceneOpened += (_, _) => EditorApplication.delayCall += TryEnsureLevel3Rooms;
    }

    private static void TryEnsureLevel3Rooms()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
        {
            EditorApplication.delayCall += TryEnsureLevel3Rooms;
            return;
        }

        if (SessionState.GetBool(BuildGuardKey, false) || !Level3AssetPuzzleBuilder.NeedsBuildInOpenScene())
        {
            return;
        }

        try
        {
            SessionState.SetBool(BuildGuardKey, true);
            Level3AssetPuzzleBuilder.EnsureRoomsBuiltInOpenLevel3(true);
            Debug.Log("Level3 auto-setup rebuilt the five-room if puzzle in the open scene.");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Level3 auto-setup failed: {exception.Message}");
        }
        finally
        {
            SessionState.SetBool(BuildGuardKey, false);
        }
    }
}
