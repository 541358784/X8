using UnityEngine;

public static class PostProcessingUtils
{
    public static T AddPostProcessing<T>(Camera camera) where T : MonoBehaviour
    {
        return camera.gameObject.GetOrCreateComponent<T>();
    }

    public static void RemovePostProcessing<T>(Camera camera) where T : MonoBehaviour
    {
        var com = camera.gameObject.GetComponent<T>();
        if (com)
        {
            GameObject.Destroy(com);
        }
    }
}