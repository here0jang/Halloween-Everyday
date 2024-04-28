using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public async void OnLoginClickedAsync()
    {
        // TODO : Addressable �� ����
        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));

        Exception exception = await LobbyManager.Authenticate();
        if(exception == null)
        {
            SceneManager.LoadScene("01 Main");
        }
        else
        {
            Destroy(loading);

            // TODO : �÷��̾ �ذ��� �� �ִ� ������ ��� �ȳ� �޽��� �߰� (�����ڵ�� �з�)
            PopUpUIManager popUp = Instantiate(Resources.Load<PopUpUIManager>("PopUp UI"));
            popUp.InstantiatePopUp("�α��� ����" + exception);
        }
    }
}
