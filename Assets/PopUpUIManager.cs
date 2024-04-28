using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUIManager : MonoBehaviour
{
    public TMPro.TMP_Text MessageText;

    public Button ConfirmButton;

    public Button YesButton;
    public Button NoButton;

    public void InstantiatePopUp(string message)
    {
        MessageText.text = message;
        ConfirmButton.onClick.AddListener(() => 
        {
            Destroy(gameObject);
        });

        YesButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);
    }

    public void InstantiatePopUp(string message, Action yesAction)
    {
        MessageText.text = message;

        YesButton.onClick.AddListener(() =>
        {
            yesAction();
        });
        NoButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });

        ConfirmButton.gameObject.SetActive(false);
    }
}
