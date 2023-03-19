using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    private GameObject actor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetActiveStats(GameObject demon)
    {
        actor = demon;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).GetComponent<Slider>().value = ((float) actor.GetComponent<ActorStats>().stats.battleStats.hp) / ((float) actor.GetComponent<ActorStats>().stats.baseStats.hp);

        if (transform.GetChild(0).GetComponent<Slider>().value == 0)
            gameObject.SetActive(false);
    }
}
