using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowNormalSceneManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text mTopicText;

    [SerializeField] private Button mExitButton;
    [SerializeField] private TMPro.TMP_Text mExitButtonText;
    [SerializeField] private Button mContinueButton;

    [SerializeField] private GameObject mQuizPrefab;

    [SerializeField] private Transform mCameraTransform;

    [SerializeField] private ItemDatabase[] mItemDatabases = new ItemDatabase[(int)EItems.last];

    private List<ulong> mConnectedClients = new List<ulong>();

    public static ShowNormalSceneManager Instance { get; private set; }
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

        mExitButton.onClick.RemoveAllListeners();
        mExitButton.onClick.AddListener(() =>
        {
            GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(SceneName.WEB_LOGIN_SCENE);
        });

        //mContinueButton.onClick.RemoveAllListeners();
        //mContinueButton.onClick.AddListener(() =>
        //{
        //    // ¥Î±‚æ¿ ¿Ãµø
        //});
    }

    public void ShowSceneInit(ulong id)
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

        startShowClientRpc();
    }

    [ClientRpc]
    private void startShowClientRpc()
    {
        mTopicText.text = MultiplayerManager.Instance.mTopic.Value.ToString();

        for (int i = 0; i < MultiplayerManager.Instance.mQuizDatas.Count; i++)
        {
            ulong quizId = MultiplayerManager.Instance.mQuizDatas[i].Id;

            GameObject quizObject = Instantiate(mQuizPrefab, new Vector3(6*i, 0, 1.98f), quaternion.identity);

            quizObject.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = MultiplayerManager.Instance.mQuizDatas[i].Quiz.ToString();
            quizObject.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = MultiplayerManager.Instance.GetAnswer(quizId);

            StyleData styleData = MultiplayerManager.Instance.GetStyle(quizId);
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[0].sharedMesh = styleData.BackpackId == -1 ? null : mItemDatabases[(int)EItems.backpack].mDatas[styleData.BackpackId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[1].sharedMesh = styleData.BodyId == -1 ? null : mItemDatabases[(int)EItems.body].mDatas[styleData.BodyId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[2].sharedMesh = styleData.EyebrowId == -1 ? null : mItemDatabases[(int)EItems.eyebrow].mDatas[styleData.EyebrowId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[3].sharedMesh = styleData.FullBodyId == -1 ? null : mItemDatabases[(int)EItems.fullBody].mDatas[styleData.FullBodyId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[4].sharedMesh = styleData.GlassesId == -1 ? null : mItemDatabases[(int)EItems.glasses].mDatas[styleData.GlassesId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[5].sharedMesh = styleData.GloveId == -1 ? null : mItemDatabases[(int)EItems.glove].mDatas[styleData.GloveId].mMesh;

            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[6].sharedMesh = styleData.HairId == -1 ? null : mItemDatabases[(int)EItems.hair].mDatas[styleData.HairId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[7].sharedMesh = styleData.HatId == -1 ? null : mItemDatabases[(int)EItems.hat].mDatas[styleData.HatId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[8].sharedMesh = styleData.MustacheId == -1 ? null : mItemDatabases[(int)EItems.mustache].mDatas[styleData.MustacheId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[9].sharedMesh = styleData.OuterId == -1 ? null : mItemDatabases[(int)EItems.outerwear].mDatas[styleData.OuterId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[10].sharedMesh = styleData.PantsId == -1 ? null : mItemDatabases[(int)EItems.pants].mDatas[styleData.PantsId].mMesh;
            quizObject.GetComponentsInChildren<SkinnedMeshRenderer>()[11].sharedMesh = styleData.ShoeId == -1 ? null : mItemDatabases[(int)EItems.shoe].mDatas[styleData.ShoeId].mMesh;
        }

        StartCoroutine(moveCamera(MultiplayerManager.Instance.mQuizDatas.Count ));

        StartCoroutine(timer());
    }

    IEnumerator moveCamera(int count)
    {
        Vector3 startPos = mCameraTransform.position;

        float elapsedTime = 0f;
        while(elapsedTime <200 )
        {
            elapsedTime += Time.deltaTime;

            Vector3 targetPos = startPos + new Vector3(count * 6 - 6, 0, 0);
            Vector3 lerpedPos = Vector3.Lerp(mCameraTransform.position, targetPos, (elapsedTime/ 200 ));
            mCameraTransform.position = lerpedPos;  

            yield return null;
        }
    }

    IEnumerator timer() 
    {
        int i = GameCount.SHOW_COUNT;
        while (i > 0) 
        {
            mExitButtonText.text = "¿⁄µø ≈¿Â " + i + "√ "; 
            i--;
            yield return new WaitForSeconds(1);
        }

        GameObject loading = Instantiate(Resources.Load<GameObject>("Loading UI"));
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneName.WEB_LOGIN_SCENE);
    }
}
