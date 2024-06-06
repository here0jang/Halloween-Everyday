using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private Button mLoginButton;

    private async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        mLoginButton.onClick.RemoveAllListeners();
        mLoginButton.onClick.AddListener(async () => 
        {
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        try
        {
            // TODO : �͸� �α��� (���� ���� �α������� ����)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            SceneManager.LoadScene(SceneName.MAIN_SCENE);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);

            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("�α��� ����");
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);

            Destroy(loading);
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
            popUp.InstantiatePopUp("�α��� ����");
        }
        });
    }
}
