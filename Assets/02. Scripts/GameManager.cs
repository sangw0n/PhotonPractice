using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        //출현 위치 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int indx = Random.Range(1, points.Length);

        // 네트워크상애 캐릭터 생성
        PhotonNetwork.Instantiate("Player", points[indx].position, points[indx].rotation, 0);
    }
}
