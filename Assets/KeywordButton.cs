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
            // 정답
            ResultText.text = "정답!";
        }
        else
        {
            // 오답
            ResultText.text = "다시 생각해보세요";
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
