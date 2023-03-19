using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemonDropdown : MonoBehaviour
{
    public GameObject demonDex;

    // Start is called before the first frame update
    public void PopulateDropDown()
    {
        List<string> optionsList = demonDex.GetComponent<DemonDex>().demonNames;
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string optionString in optionsList)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(optionString);
            options.Add(option);
        }

        // Add the options to the dropdown
        gameObject.GetComponent<Dropdown>().AddOptions(options);
    }
}
