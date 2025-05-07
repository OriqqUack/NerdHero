using UnityEngine;
using BackEnd;
using LitJson;

public class BackendDataManager
{
    public int score, gold, gem, highScore;

    public void LoadUserData()
    {
        var bro = Backend.GameData.GetMyData("UserData", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogError("데이터 불러오기 실패: " + bro.GetMessage());
            return;
        }

        JsonData rows = bro.FlattenRows();

        if (rows.Count == 0)
        {
            // 신규 유저: 기본값 세팅 후 Insert
            score = 0;
            gold = 100;
            gem = 10;
            highScore = 0;
            InsertNewUserData();
        }
        else
        {
            // 기존 유저: 값 불러오기
            score = int.Parse(rows[0]["score"].ToString());
            gold = int.Parse(rows[0]["gold"].ToString());
            gem = int.Parse(rows[0]["gem"].ToString());
            highScore = int.Parse(rows[0]["highScore"].ToString());

            Debug.Log("기존 유저 데이터 로드 완료");
        }
    }

    void InsertNewUserData()
    {
        Param param = new Param();
        param.Add("score", score);
        param.Add("gold", gold);
        param.Add("gem", gem);
        param.Add("highScore", highScore);

        var bro = Backend.GameData.Insert("UserData", param);

        if (bro.IsSuccess())
        {
            Debug.Log("신규 유저 데이터 저장 성공");
        }
        else
        {
            Debug.LogError("데이터 저장 실패: " + bro.GetMessage());
        }
    }

    public void UpdateUserData()
    {
        Param param = new Param();
        param.Add("score", score);
        param.Add("gold", gold);
        param.Add("gem", gem);
        param.Add("highScore", highScore);

        Where where = new Where();
        where.Equal("owner_inDate", Backend.UserInDate);

        var bro = Backend.GameData.Update("UserData", where, param);

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 업데이트 성공");
        }
        else
        {
            Debug.LogError("업데이트 실패: " + bro.GetMessage());
        }
    }
}
