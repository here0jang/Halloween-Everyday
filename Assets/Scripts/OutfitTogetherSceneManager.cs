//using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;

public class OutfitTogetherSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mTimerText;
    [SerializeField] private TMPro.TMP_Text mKeywordText;
    //[SerializeField] private TMPro.TMP_Text mDescText;
    [SerializeField] private GameObject mLoading;

    //[SerializeField] private CustomizationMediator mCustomizationMediator;

    private List<ulong> mConnectedClients = new List<ulong>();


    public static OutfitTogetherSceneManager Instance { get; private set; }
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

        Debug.Log("��� �÷��̾� ���� " + mConnectedClients.Count);
        StartOutFitClientRpc();
    }

    [ClientRpc]
    private void StartOutFitClientRpc()
    {
        startOutfitAsync();
    }

    private async void startOutfitAsync()
    {
        // �ε�
        mLoading.SetActive(false);


        // ù��° Ű���� ��������
        mKeywordText.text = "�Ӹ��� ���� �Ұ�";
        PlayerManager mPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        //mPlayerManager.GetKeywordServerRpc();
        //await Task.Delay(3000);
        //mKeywordText.text = "<color=#6D60CC>" + mPlayerManager.mCurKeyword.Value + "</color>�� �ٹ̼���!";
        //mDescText.text = "�Ӹ��� ���� �Ұ�";


        // Ÿ�̸� ����
        float timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mTimerText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // �ε�
        mLoading.SetActive(true);


        // �Ӹ� ��Ÿ�� ����
        //string styleId = mCustomizationMediator.SaveSettings();
        //mPlayerManager.SetCurStyleServerRpc(styleId);

        // �ε�
        mLoading.SetActive(false);


        // �ι�° Ű���� & ��Ÿ�� ��������
        mKeywordText.text = "���� ���� ������";
        //mPlayerManager.GetKeywordServerRpc();
        //mPlayerManager.GetStyleServerRpc();
        //await Task.Delay(3000);
        //mKeywordText.text = "<color=#6D60CC>" + mPlayerManager.mCurKeyword.Value + "</color>�� �ٹ̼���!";
        //mCustomizationMediator.LoadSettings(mPlayerManager.mCurStyle.Value.ToString());
        //mDescText.text = "���� ���� ������";

        // �Ӹ� ��ư ����




        // Ÿ�̸� ����
        timer = (float)System.DateTime.Now.TimeOfDay.TotalSeconds + GameCount.OUTFIT_COUNT; /* �ϵ���� �ð� �̿� */
        while (timer > (float)System.DateTime.Now.TimeOfDay.TotalSeconds)
        {
            mTimerText.text = $"{timer - (float)System.DateTime.Now.TimeOfDay.TotalSeconds:N0}";
            await Task.Yield();
        }

        // �ι�° ��Ÿ�� ����

        // ���� �̵�
    }
}
