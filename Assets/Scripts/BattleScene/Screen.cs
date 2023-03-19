using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public GameObject gameManager;
    public Canvas healthBarOutline;
    public Image resistText;
    // public GameObject pressTurns;
    // public Image pressTurnIcon;
    // public Canvas entityScreen;

    // Start is called before the first frame update
    void Start()
    {
        // RenderPressTurns();

        StartCoroutine(SetHealthBars());
        StartCoroutine(SetResistText());
    }

    IEnumerator SetHealthBars()
    {
        yield return new WaitForSeconds(0.1f);

        CreateHealthBar(gameManager.GetComponent<GameManager>().playerTeam.GetComponent<Team>().player);
        foreach(GameObject child in gameManager.GetComponent<GameManager>().playerTeam.GetComponent<Team>().activeDemons)
        {
            CreateHealthBar(child);
        }

        CreateHealthBar(gameManager.GetComponent<GameManager>().opponentTeam.GetComponent<Team>().player);
        foreach(GameObject child in gameManager.GetComponent<GameManager>().opponentTeam.GetComponent<Team>().activeDemons)
        {
            CreateHealthBar(child);
        }
    }

    IEnumerator SetResistText()
    {
        yield return new WaitForSeconds(0.1f);

        CreateResistText(gameManager.GetComponent<GameManager>().playerTeam.GetComponent<Team>().player);
        foreach(GameObject child in gameManager.GetComponent<GameManager>().playerTeam.GetComponent<Team>().activeDemons)
        {
            CreateResistText(child);
        }

        CreateResistText(gameManager.GetComponent<GameManager>().opponentTeam.GetComponent<Team>().player);
        foreach(GameObject child in gameManager.GetComponent<GameManager>().opponentTeam.GetComponent<Team>().activeDemons)
        {
            CreateResistText(child);
        }
    }

    void CreateHealthBar(GameObject child)
    {
        Transform obj = child.transform;
        var healthBar = Instantiate(healthBarOutline, obj.position, Quaternion.identity);
        healthBar.GetComponent<HealthBar>().GetActiveStats(child);
        healthBar.transform.SetParent(this.transform.Find("HealthBars"));
        healthBar.GetComponent<EntityHUD>().lookAt = obj;
    }

    void CreateResistText(GameObject child)
    {
        Transform obj = child.transform;
        var resistImage = Instantiate(resistText, obj.position, Quaternion.identity);
        resistImage.GetComponent<ResistText>().GetActiveStats(child);
        resistImage.GetComponent<Image>().enabled = false;
        resistImage.transform.SetParent(this.transform.Find("ResistMessages"));
        resistImage.GetComponent<EntityHUD>().lookAt = obj;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void RenderPressTurns()
    // {
    //     CreatePressTurn(0);
    // }

    // void CreatePressTurn(int offset)
    // {
    //     var pressTurn = Instantiate(pressTurnIcon, Vector3.zero, Quaternion.identity) as Image;
    //     pressTurn.transform.SetParent(transform);
    //     pressTurn.color = Color.cyan;
    //     var rectTransform = pressTurn.GetComponent<RectTransform>();
    //     rectTransform.anchoredPosition = new Vector2(1, 0);
    //     rectTransform.position = new Vector3(1830 - (100 * offset), 990, 0);
    // }
}
