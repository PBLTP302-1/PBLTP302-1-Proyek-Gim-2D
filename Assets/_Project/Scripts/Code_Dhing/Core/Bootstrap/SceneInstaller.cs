using System.Collections.Generic;
using UnityEngine;

public abstract class SceneInstaller : MonoBehaviour
{
    public abstract void Install(GameServices services);

    protected IEnumerable<T> FindInScene<T>()
    {
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
            foreach (T component in root.GetComponentsInChildren<T>(true))
                yield return component;
    }
}
