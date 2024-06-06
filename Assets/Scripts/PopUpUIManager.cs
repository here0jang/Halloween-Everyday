using System;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUIManager : MonoBehaviour
{
    public const string RESOURCE_NAME = "PopUp UI";

    [SerializeField] private TMPro.TMP_Text mMessageText;

    [Header("1 Button Popup")]
    [SerializeField] private Button mConfirmButton;

    [Header("2 Button Popup")]
    [SerializeField] private Button mYesButton;
    [SerializeField] private Button mNoButton;

    public void InstantiatePopUp(string message)
    {
        mMessageText.text = message;
        mConfirmButton.onClick.AddListener(() => 
        {
            Destroy(gameObject);
        });

        mYesButton.gameObject.SetActive(false);
        mNoButton.gameObject.SetActive(false);
    }

    public void InstantiatePopUp(string message, Action yesAction)
    {
        mMessageText.text = message;

        mYesButton.onClick.AddListener(() =>
        {
            yesAction();
            Destroy(gameObject);
        });
        mNoButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });

        mConfirmButton.gameObject.SetActive(false);
    }
}
    