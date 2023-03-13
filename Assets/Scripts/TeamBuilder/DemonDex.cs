using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonDex : MonoBehaviour
{
    public List<GameObject> demonDex = new List<GameObject>();

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
