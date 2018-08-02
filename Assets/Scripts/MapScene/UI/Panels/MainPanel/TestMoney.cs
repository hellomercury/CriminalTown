using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestMoney : MonoBehaviour
{
    public InputField moneyInputField;

    private void Start()
    {
        moneyInputField.text = DataScript.SData.Money.ToString();
    }

    private void Update()
    {
        moneyInputField.text = DataScript.SData.Money.ToString();
    }

    public void OnEndEditMoney()
    {
        int.TryParse(moneyInputField.text, out DataScript.SData.Money);
        DataScript.SaveSourcesData();
    }
}
