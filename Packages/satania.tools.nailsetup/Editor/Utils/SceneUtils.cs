using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace satania.shopping.tool
{
    internal static class SceneUtils
    {
        internal static bool IsSceneValidAndNotPreviewScene(Scene scene)
        {
            if (scene == null) return false;
            return !EditorSceneManager.IsPreviewScene(scene) && scene.IsValid();
        }

        internal static bool IsSceneValidAndNotPreviewScene(GameObject go)
        {
            if (go == null) return false;
            return IsSceneValidAndNotPreviewScene(go.scene);
        }

        internal static bool IsSceneValidAndNotPreviewScene(Transform transform)
        {
            if (transform == null) return false;
            return IsSceneValidAndNotPreviewScene(transform.gameObject.scene);
        }
    }
}