using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResistText : MonoBehaviour
{
    public Sprite weakText;
    public Sprite criticalText;
    public Sprite missText;
    public Sprite resistText;
    public Sprite nullText;
    public Sprite guardText;
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
        if (actor.GetComponent<ActorStats>().stats.damageStatus != 0)
            StartCoroutine(ShowDamageText());
    }

    IEnumerator ShowDamageText()
    {
        GetComponent<Image>().enabled = true;
        switch(actor.GetComponent<ActorStats>().stats.damageStatus)
        {
            case 1:
                GetComponent<Image>().sprite = weakText;
                break;
            case 2:
                GetComponent<Image>().sprite = criticalText;
                break;
            case 3:
                GetComponent<Image>().sprite = missText;
                break;
            case 4:
                GetComponent<Image>().sprite = resistText;
                break;
            case 5:
                GetComponent<Image>().sprite = nullText;
                break;
        }
        yield return new WaitForSeconds(0.8f);
        actor.GetComponent<ActorStats>().stats.damageStatus = 0;
        GetComponent<Image>().enabled = false;
    }
}
