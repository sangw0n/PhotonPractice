using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Photon
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 게임의 버전
    private readonly string verison = "1.0";
    // 유저의 닉네임
    private string userId = "Zack";

    // 유저명을 입력할 TextMeshPro Input Field 
    public TMP_InputField userIF;
    // 룸 이름을 입력할 TextMeshPro Input Field
    public TMP_InputField roomNameIf;

    private void Awake()
    {
        // 마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        // 게임 버전 설정
        PhotonNetwork.GameVersion = verison;
        // 접속 유저의 닉네임 설정
        // PhotonNetwork.NickName = userId;

        // 포톤 서버와의 데이터의 초당 전송 횟수
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        // 저장된 유저명을 로드
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userId;
        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName = userId;
    }

    // 유저명을 설정하는 로직
    public void SetUserId()
    {
        if(string.IsNullOrEmpty(userIF.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else userId = userIF.text;
        // 유저명 저장
        PlayerPrefs.SetString("USER_ID", userIF.text);
        // 접속 유저의 닉네임 등록
        PhotonNetwork.NickName = userId;
    }

    // 룸 명의 입력 여부를 확인하는 로직
    private string SetRoomName()
    {
        if (string.IsNullOrEmpty(roomNameIf.text))
        {
            roomNameIf.text = $"ROOM_{Random.Range(1, 101): 000}";
        }

        return roomNameIf.text;
    }

    // 포톤 서버에 접속 후 호출되는 콜백함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connectd to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotoNetwork.Inlooby = {PhotonNetwork.InLobby}");
        // 수동으로 접속하기 위해 자동 입장인 주석처리
        // PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수 
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        // 룸을 생성하는 함수 시행
        OnMakeRoomClick();

        // 룸의 속성 정의
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 20; // 룸에 입장할 수 있는 최대 접속자 수 
        //ro.IsOpen = true; // 룸의 오픈 여부
        //ro.IsVisible = true; // 로비에서 룸 목록에 노출시킬지 여부

        // 룸 생성
        // PhotonNetwork.CreateRoom("My Room", ro);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        // // 출현 위치 정보를 배열에 저장
        // Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        // int idx = Random.Range(1, points.Length);

        // // 네트워크상에 캐릭터 생성
        // PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        // 마스터 클라이언트인 경우에 룸에 입장한 후 전투 씬을 로딩한다
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    #region  UI_BUTTON_EVENT

    public void OnLoginClick()
    {
        // 유저명 저장
        SetUserId();

        // 무작위로 추출한 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        // 유저명 저장
        SetUserId();

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20; // 룸에 입장할 수 있는 최대 접속자 수
        ro.IsOpen = true; // 룸의 오픈 여부
        ro.IsVisible = true; // 로비에서룸 목록에 노출시킬 지 여부

        // 룸 생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
    #endregion
}

