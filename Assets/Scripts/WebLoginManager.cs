using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class WebLoginManager : MonoBehaviour
{
    [SerializeField] private GameObject mMultiPlayerManagerPrefab;

    [Header("[UI]")]
    [SerializeField] private TMPro.TMP_InputField mNicNameInput;
    [SerializeField] private TMPro.TMP_InputField mJoinCodeInput;
    [SerializeField] private Button mJoinButton;
    [SerializeField] private Button mCreateButton;

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

        #region Login
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                // TODO : 익명 로그인 (추후 구글 로그인으로 변경)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
                popUp.InstantiatePopUp("로그인 실패");
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
                popUp.InstantiatePopUp("로그인 실패");
            }
        }
        #endregion

        mNicNameInput.text = "내별명" + UnityEngine.Random.Range(1, 100);

        mJoinButton.onClick.RemoveAllListeners();
        mJoinButton.onClick.AddListener(async () =>
        {
            #region Client
            if (mJoinCodeInput.text == string.Empty)
            {
                PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
                popUp.InstantiatePopUp("입장 코드를 입력하세요");
                return;
            }


            GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
            setName();

            bool joined = await LobbyManager.StartClientWithRelay(mJoinCodeInput.text);
            if (!joined)
            {
                Destroy(loading);
                PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>(PopUpUIManager.RESOURCE_NAME));
                popUp.InstantiatePopUp("방 입장에 실패했습니다");
                return;
            }
            #endregion
        });

        mCreateButton.onClick.RemoveAllListeners();
        mCreateButton.onClick.AddListener(async () =>
        {
            #region Host
            GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
            string code = await LobbyManager.StartHostWithRelay();

            setName();
            PlayerPrefs.SetString("Code", code);

            GameObject multiPlayer = Instantiate(mMultiPlayerManagerPrefab, Vector3.zero, Quaternion.identity);
            multiPlayer.GetComponent<NetworkObject>().Spawn();

            LoadingSceneManager.Instance.LoadScene(SceneName.WAITING_SCENE, true);
            #endregion
        });
    }

    private void setName()
    {
        if (mNicNameInput.text == string.Empty)
        {
            mNicNameInput.text = "내별명" + UnityEngine.Random.Range(1, 100);
        }
        PlayerPrefs.SetString("Name", mNicNameInput.text);
    }
}
