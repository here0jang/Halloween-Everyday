using System;
using Unity.Collections;
using Unity.Netcode;

//public struct GameData
//{
//    public ulong ClientId;
//    public FixedString32Bytes NicName;

//    public FixedString32Bytes Keyword;
//    public List<StyleData> Styles;
//    public List<AnswerData> Answers;
//}

//public struct AnswerData
//{
//    public ulong ClientId;
//    public FixedString32Bytes NicName;

//    public FixedString32Bytes Answer;
//}

//public struct StyleData
//{
//    public ulong ClientId;
//    public FixedString32Bytes NicName;

//    // head
//    public int HairId;
//    public int EyebrowId;
//    public int MustacheId;
//    public int BodyId;
//    public int GlassesId;
//    public int HatId;

//    // body
//    public int FullBodyId;
//    public int OuterId;
//    public int PantsId;
//    public int BackpackId;
//    public int GloveId;
//    public int ShoeId;
//}


public struct QuizData : IEquatable<QuizData>, INetworkSerializable
{
    public ulong QuizId;
    public FixedString32Bytes NicName;

    public FixedString32Bytes Quiz;

    public bool Equals(QuizData other)
    {
        return QuizId == other.QuizId
            && NicName == other.NicName
            && Quiz == other.Quiz;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref QuizId);
        serializer.SerializeValue(ref NicName);

        serializer.SerializeValue(ref Quiz);
    }
}

public struct StyleData : IEquatable<StyleData>, INetworkSerializable
{
    public ulong QuizId;
    public ulong MyId;

    // head
    public int HairId;
    public int EyebrowId;
    public int MustacheId;
    public int BodyId;
    public int GlassesId;
    public int HatId;

    // body
    public int FullBodyId;
    public int OuterId;
    public int PantsId;
    public int BackpackId;
    public int GloveId;
    public int ShoeId;

    public bool Equals(StyleData other)
    {
        return QuizId == other.QuizId
            && MyId == other.MyId
            && HairId == other.HairId
            && EyebrowId == other.EyebrowId
            && MustacheId == other.MustacheId
            && BodyId == other.BodyId
            && GlassesId == other.GlassesId
            && HatId == other.HatId
            && FullBodyId == other.FullBodyId
            && OuterId == other.OuterId
            && PantsId == other.PantsId
            && BackpackId == other.BackpackId
            && GloveId == other.GloveId
            && ShoeId == other.ShoeId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref QuizId);
        serializer.SerializeValue(ref MyId);

        serializer.SerializeValue(ref HairId);
        serializer.SerializeValue(ref EyebrowId);
        serializer.SerializeValue(ref MustacheId);
        serializer.SerializeValue(ref BodyId);
        serializer.SerializeValue(ref GlassesId);
        serializer.SerializeValue(ref HatId);

        serializer.SerializeValue(ref FullBodyId);
        serializer.SerializeValue(ref OuterId);
        serializer.SerializeValue(ref PantsId);
        serializer.SerializeValue(ref BackpackId);
        serializer.SerializeValue(ref GloveId);
        serializer.SerializeValue(ref ShoeId);
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
    public NetworkVariable<int> mConnectedCount;

    public NetworkList<QuizData> mQuizDatas;

    public NetworkList<StyleData> mStyleDatas;
    public NetworkList<AnswerData> mAnswerDatas;



    public static MultiplayerManager Instance { get; private set; }
    private void Awake() 
    {
        Instance = this;
        DontDestroyOnLoad(this);

        mQuizDatas = new NetworkList<QuizData>();
        mStyleDatas = new NetworkList<StyleData>();
        mAnswerDatas = new NetworkList<AnswerData>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddDataServerRpc(ulong myId, string quiz)
    {
        mQuizDatas.Add(new QuizData { QuizId = myId, Quiz = quiz});

        mAnswerDatas.Add(new AnswerData { QuizId = myId, MyId = myId, Answer = quiz });

        mConnectedCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
    }




    [ServerRpc(RequireOwnership = false)]
    public void AddAnswerServerRpc(ulong quizId, ulong myId, string answer)
    {
        mAnswerDatas.Add(new AnswerData { QuizId = quizId, MyId = myId, Answer = answer});
    }

    public string GetAnswer(int index)
    {
        foreach (AnswerData answerData in mAnswerDatas)
        {
            if ((int)answerData.MyId == index)
            {
                return answerData.Answer.ToString();
            }
        }

        return null;
    }






    [ServerRpc(RequireOwnership = false)]
    public void AddStyleServerRpc(ulong quizId, ulong id, int hair)
    {
        mStyleDatas.Add(new StyleData {  QuizId = quizId, MyId = id,  HairId = hair});
    }

    public StyleData GetStyle(int index)
    {
        //int index = (myIndex + 1) % mConnectedCount.Value;

        foreach (StyleData playerData in mStyleDatas)
        {
            if ((int)playerData.QuizId == index)
            {
                return playerData;
            }
        }

        return mStyleDatas[0];
    }
}
