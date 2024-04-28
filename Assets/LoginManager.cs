using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public async void OnLoginClickedAsync()
    {
        // TODO : Addressable 로 변경
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        Exception exception = await LobbyManager.Authenticate();
        if(exception == null)
        {
            SceneManager.LoadScene("01 Main");
        }
        else
        {
            Destroy(loading);

            // TODO : 플레이어가 해결할 수 있는 원인일 경우 안내 메시지 추가 (에러코드로 분류)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("로그인 실패" + exception);
        }
    }
}
