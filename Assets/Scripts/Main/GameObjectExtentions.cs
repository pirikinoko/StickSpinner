using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class GameObjectExtentions
{
    public static List<GameObject> FindGameObjectsWithTagIncludingInactive(string tag)
    {
        List<GameObject> results = new List<GameObject>();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) && obj.hideFlags != HideFlags.NotEditable && obj.hideFlags != HideFlags.HideAndDontSave)
            {
                results.Add(obj);
            }
        }

        return results;
    }

    public static GameObject FindObjectWithContainingName(string name)
    {
        GameObject[] target = GameObject.FindObjectsOfType<GameObject>()
       .Where(go => go.name.Contains(name))
       .ToArray();
        return target[0];
    }
}
