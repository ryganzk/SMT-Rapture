using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DemonDex : MonoBehaviour
{
    public List<GameObject> demonDex = new List<GameObject>();
    public List<string> demonNames = new List<string>();

    void Start()
    {
        LoadDemons();
    }

    private void LoadDemons()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Actors" });

        // Load each prefab and add it to the list
        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(path));
            if (prefab != null)
            {
                demonDex.Add(prefab);
                demonNames.Add(char.ToUpper(prefab.name[0]) + prefab.name.Substring(1));
            }
        }
    }

    public GameObject MatchNameWithDemon(string name)
    {
        foreach (GameObject demon in demonDex)
        {
            if (name == demon.GetComponent<ActorStats>().stats.name)
            {
                return demon;
            }
        }
        return null;
    }

    public GameObject MatchIndexWithDemon(int index)
    {
        return demonDex[index];
    }
}
