using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string Topic;
    public List<string> Keywords;
}

[CreateAssetMenu(fileName = "TopicKeyword", menuName = "ScriptableObjects/TopicKeyword", order = 1)]
public class TopicKeywordData : ScriptableObject
{
    public List<Item> Items;
}
