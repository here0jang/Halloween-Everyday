using UnityEngine;
using System.Threading.Tasks;
//using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class OutfitManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mLimitText;
    //[SerializeField] private TMPro.TMP_Text mNicNameText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;
    [SerializeField] private GameObject mLoading;

    //[SerializeField] private CustomizationMediator mCustomizationMediator;

    private List<ulong> mConnectedClients = new List<ulong>();


    public static OutfitManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OutfitSceneInit(ulong id)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            return;
        }

        mConnectedClients.Add(id);

        if (mConnectedClients.Count < NetworkManager.Singleton.ConnectedClients.Count)
        {
            return;
        }

        Debug.Log("��� �÷��̾� ���� "  + mConnectedClients.Count);
        StartOutFitClientRpc();
    }

    [ClientRpc]
    private void StartOutFitClientRpc()
    {
        startOutfitAsync();
    }

    private async void startOutfitAsync()
    {
        // Ű����, �г���
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.GetKeywordServerRpc();
        await Task.Delay(3000);

        mLoading.SetActive(false);
        //mKey//wordText.text = "<color=#6D60CC>" + mPlayerManager.mCurKeyword.Value + "</color>�� �ٹ̼���!";
        //NicNameText.text = mPlayerManager.mCurNicName.Value + "<color=#6D60CC>�� ����</color>";


        // Ÿ�̸�
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mLimitText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }


        // ��Ÿ�� ����
        mLoading.SetActive(true);
        //string styleId = mCustomizationMediator.SaveSettings();
        //mPlayerManager.SetCurStyleServerRpc(styleId);


        // ���� �̵�
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("05 QUIZ", LoadSceneMode.Single);
        }
    }
}
