using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public GameObject pressTurns;
    public Image pressTurnIcon;

    // Start is called before the first frame update
    void Start()
    {
        RenderPressTurns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderPressTurns()
    {
        CreatePressTurn(0);
    }

    void CreatePressTurn(int offset)
    {
        var pressTurn = Instantiate(pressTurnIcon, Vector3.zero, Quaternion.identity) as Image;
        pressTurn.transform.SetParent(transform);
        pressTurn.color = Color.cyan;
        var rectTransform = pressTurn.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(1, 0);
        rectTransform.position = new Vector3(1830 - (100 * offset), 990, 0);
    }
}
