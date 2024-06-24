using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct NameData : IEquatable<NameData>, INetworkSerializable
{
    public ulong Id;
    public FixedString32Bytes Name;

    public bool Equals(NameData other)
    {
        return Id == other.Id && Name == other.Name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Name);
    }
}
public struct QuizData : IEquatable<QuizData>, INetworkSerializable
{
    public ulong Id;
    public FixedString32Bytes Quiz;

    public bool Equals(QuizData other)
    {
        return Id == other.Id && Quiz == other.Quiz;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Quiz);
    }
}
public struct StyleData : IEquatable<StyleData>, INetworkSerializable
{
    public ulong QuizId;
    public ulong MyId;

    // head
    public int HairId;
    public int BodyId;
    public int EyebrowId;
    public int MustacheId;
    public int GlassesId;
    public int HatId;

    // body
    public int OuterId;
    public int PantsId;
    public int ShoeId;
    public int GloveId;
    public int BackpackId;
    public int FullBodyId;


    public bool Equals(StyleData other)
    {
        return QuizId == other.QuizId
            && MyId == other.MyId
            && HairId == other.HairId
            && BodyId == other.BodyId
            && EyebrowId == other.EyebrowId
            && MustacheId == other.MustacheId
            && GlassesId == other.GlassesId
            && HatId == other.HatId
            && OuterId == other.OuterId
            && PantsId == other.PantsId
            && ShoeId == other.ShoeId
            && GloveId == other.GloveId
            && BackpackId == other.BackpackId
            && FullBodyId == other.FullBodyId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref QuizId);
        serializer.SerializeValue(ref MyId);

        serializer.SerializeValue(ref HairId);
        serializer.SerializeValue(ref BodyId);
        serializer.SerializeValue(ref EyebrowId);
        serializer.SerializeValue(ref MustacheId);
        serializer.SerializeValue(ref GlassesId);
        serializer.SerializeValue(ref HatId);

        serializer.SerializeValue(ref OuterId);
        serializer.SerializeValue(ref PantsId);
        serializer.SerializeValue(ref ShoeId);
        serializer.SerializeValue(ref ShoeId);
        serializer.SerializeValue(ref BackpackId);
        serializer.SerializeValue(ref FullBodyId);
    }
}
public struct AnswerData : IEquatable<AnswerData>, INetworkSerializable
{
    public ulong QuizId;
    public ulong MyId;

    public FixedString32Bytes Answer;

    public bool Equals(AnswerData other)
    {
        return QuizId == other.QuizId
            && MyId == other.MyId
            && Answer == other.Answer;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref QuizId);
        serializer.SerializeValue(ref MyId);

        serializer.SerializeValue(ref Answer);
    }
}

public class MultiplayerManager : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> mRelayCode;
    public NetworkVariable<FixedString32Bytes> mTopic;
    [SerializeField] private NetworkVariable<int> mTopicIndex;
    public int TopicIndex
    {
        get 
        {
            return mTopicIndex.Value;
        }
        set 
        {
            if(IsHost)
            {
                mTopicIndex.Value = value;
            }
        }
    }
    public NetworkVariable<FixedString32Bytes> mGameMode;
    public NetworkVariable<int> mConnectedCount;
    public int ConnectedCount
    {
        get
        {
            return mConnectedCount.Value;
        }
        set
        {
            mConnectedCount.Value = value;
        }
    }

    public NetworkList<NameData> mNameDatas;
    public NetworkList<QuizData> mQuizDatas;
    public NetworkList<StyleData> mStyleDatas;
    public NetworkList<AnswerData> mAnswerDatas;

    public static MultiplayerManager Instance { get; private set; }
    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        mNameDatas = new NetworkList<NameData>();
        mQuizDatas = new NetworkList<QuizData>();
        mStyleDatas = new NetworkList<StyleData>();
        mAnswerDatas = new NetworkList<AnswerData>();
    }
    public override void OnNetworkSpawn()
    {
        if(IsHost)
        {
            mConnectedCount.Value = 0;
        }

        mConnectedCount.OnValueChanged += (int prevVal, int newVal) =>
        {
            WaitingSceneManager.Instance.UpdatePlayerCount(newVal);
        };

        mNameDatas.OnListChanged+= OnNameDatasChanged;
    }

    public string GetGameMode()
    {
        return mGameMode.Value.ToString();
    }

    #region Name
    private void OnNameDatasChanged(NetworkListEvent<NameData> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<NameData>.EventType.Add)
        {
            WaitingSceneManager.Instance.ShowEnter(changeEvent.Value.Name.ToString());
        }

        WaitingSceneManager.Instance.UpdatePlayerList();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddNameServerRpc(ulong id, string name)
    {
        mNameDatas.Add(new NameData { Id = id, Name = name });
    }

    public void RemoveDisconnectedPlayerName(ulong id)
    {
        for (int i = 0; i < mNameDatas.Count; i++)
        {
            if (mNameDatas[i].Id == id)
            {
                mNameDatas.Remove(mNameDatas[i]);
                return;
            }
        }
    }

    public List<string> GetNames()
    {
        List<string> names = new List<string>();
        foreach (var item in mNameDatas)
        {
            names.Add(item.Name.ToString());
        }

        return names;
    }
    #endregion

    #region Quiz
    [ServerRpc(RequireOwnership = false)]
    public void AddQuizServerRpc(ulong id, string quiz)
    {
        mQuizDatas.Add(new QuizData { Id = id, Quiz = quiz });

        mConnectedCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
    }

    public string GetQuiz(ulong index)
    {
        foreach (QuizData quizData in mQuizDatas)
        {
            if (quizData.Id == index)
            {
                return quizData.Quiz.ToString();
            }
        }

        return null;
    }
    #endregion





    [ServerRpc(RequireOwnership = false)]
    public void AddAnswerServerRpc(ulong quizId, ulong myId, string answer)
    {
        mAnswerDatas.Add(new AnswerData { QuizId = quizId, MyId = myId, Answer = answer});
    }
    public string GetAnswer(ulong index)
    {
        foreach (AnswerData answerData in mAnswerDatas)
        {
            if (answerData.QuizId == index)
            {
                return answerData.Answer.ToString();
            }
        }

        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddStyleServerRpc(ulong quizId, ulong id, int hair, int body, int eyebrow, int mustache,  int glasses, int hat, int outer, int pants, int shoe, int glove, int backpack, int fullBody)
    {
        mStyleDatas.Add(new StyleData 
        {  
            QuizId = quizId, MyId = id,  

            HairId = hair,
            BodyId = body,
            EyebrowId = eyebrow,
            MustacheId = mustache,
            GlassesId = glasses,
            HatId = hat,
            OuterId = outer,
            PantsId = pants,
            ShoeId = shoe,        
            GloveId = glove,
            BackpackId = backpack,
            FullBodyId = fullBody,
        });
    }
    public StyleData GetStyle(ulong index)
    {
        foreach (StyleData styleData in mStyleDatas)
        {
            if (styleData.QuizId == index)
            {
                return styleData;
            }
        }

        return mStyleDatas[0];
    }
}
