using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectOrderResultUI : MonoBehaviour
{
    public int player = 1;
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public string Nickname { get; private set; }
    private string state;
    private Color myColor;

    // 닉네임 : 상태 (준비중 / 내차례 / 거리)

    private void Awake()
    {
        bgImage = GetComponent<Image>();
    }

    private void Start()
    {
        bgImage.color = Color.gray;
        
        switch(player)  //플레이어 색상
        {
            case 1: myColor = Color.red; break;
            case 2: myColor = Color.yellow; break;
            case 3: myColor = Color.green; break;
            case 4: myColor = Color.blue; break;
            default: myColor = Color.gray; break;
        }

        stateText.color = myColor;
        scoreText.gameObject.SetActive(false);

        SetReady();
    }

    /// <summary>
    /// 갱신된 데이터를 텍스트에 적용
    /// </summary>
    private void ApplyStateText()
    {
        stateText.text = $"{Nickname} : {state}";
    }
    /// <summary>
    /// 준비상태 텍스트
    /// </summary>
    public void SetReady()
    { 
        state = "준비";
        ApplyStateText();
    }
    /// <summary>
    /// 내 차례 텍스트
    /// </summary>
    public void SetMyTurn()
    {
        state = "내 차례";
        bgImage.color = myColor;
        stateText.color = Color.white;
        ApplyStateText();
    }
    /// <summary>
    /// 발사 OK
    /// </summary>
    public void SetFinish()
    {
        state = "OK";
        ApplyStateText();
    }
    /// <summary>
    /// 던진 결과를 랭킹 표시
    /// </summary>
    /// <param name="rank">다트의 Rank</param>
    public void SetRank(int rank)
    {
        string ch = "";
        switch(rank)
        {
            case 1: ch = "st"; break;
            case 2: ch = "nd"; break;
            case 3: ch = "rd"; break;
            case 4: ch = "th"; break;
        }
        state = rank.ToString() + ch;
        ApplyStateText();
    }
    /// <summary>
    /// 던진 거의 거리를 보여줌
    /// </summary>
    /// <param name="dist">다트의 Distance</param>
    public void SetScore(float dist)
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = dist.ToString("N4");
    }
}