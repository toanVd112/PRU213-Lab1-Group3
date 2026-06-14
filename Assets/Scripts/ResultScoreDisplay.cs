using TMPro;
using UnityEngine;

public class ResultScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text finalScoreText;

    private void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);

        if (finalScoreText != null)
        {
            finalScoreText.text = "TỔNG ĐIỂM: " + finalScore;
        }
    }
}