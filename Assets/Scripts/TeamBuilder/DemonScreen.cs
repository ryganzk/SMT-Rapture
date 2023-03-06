using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemonScreen : MonoBehaviour
{
    public GameObject demonDisplay;
    public GameObject demonDex;

    private DemonDex demonList;

    // Start is called before the first frame update
    void Start()
    {
        demonList = demonDex.GetComponent<DemonDex>(); 
    }

    // Update is called once per frame
    void Update()
    {
        var allyDemon = demonList.MatchIndexWithDemon(transform.Find("DemonType").GetComponent<Dropdown>().value);
        if (!demonDisplay.GetComponent<ActorStats>() || demonDisplay.GetComponent<ActorStats>().statSheet != allyDemon.GetComponent<ActorStats>().statSheet)
        {
            Destroy(demonDisplay);
            demonDisplay = Instantiate(allyDemon, new Vector3(3f, -1f, -5f), Quaternion.identity);
            demonDisplay.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX |RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            demonDisplay.transform.SetParent(transform);
            demonDisplay.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        demonDisplay.transform.Rotate(0, -30 * Time.deltaTime, 0);
    }
}
