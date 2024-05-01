using UnityEngine;

public class KeywordButton : MonoBehaviour
{
    private string mKeyword = "";
    private string mAnswer = "";

    public TMPro.TMP_Text ButtonText;

    public GameObject Result;
    public TMPro.TMP_Text ResultText;

    public void OnKeywordClicked()
    {
        if(mKeyword == mAnswer)
        {
            // ����
            ResultText.text = "����!";
        }
        else
        {
            // ����
            ResultText.text = "�ٽ� �����غ�����";
        }

        Result.SetActive(true);
    }

    public void SetKeywordButton(string keyword, string answer)
    {
        mKeyword = keyword;
        mAnswer = answer;

        ButtonText.text = mKeyword;
    }
}
