using UnityEngine;
using UnityEngine.SceneManagement;

public class FindScene
{
    public static Transform FindSceneRoot(string rootName)
    {
        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (rootName == rootObject.transform.name) return rootObject.transform;
        }
        return null;
    }
}
