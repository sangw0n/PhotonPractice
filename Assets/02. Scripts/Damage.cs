using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    // 사망 후 투명 처리를 위한 MeshRenderer 컴포넌트 배열
    private Renderer[] renderers;

    // 캐릭터의 초기 생명치 
    private int initHp = 100;
    // 캐릭터의 현재 생명치
    private int curHp = 100;

    private Animator anim;
    private CharacterController cc;

    // 애니메이터 뷰에 생성한 파라미터의 해시값 추출
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");

    private void Awake() {
        // 캐릭터 모델의 모든 Renderer 컴포넌트를 추출한 후 배열에 할당
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        // 현재 생명치를 초기 생명치로 초깃값 설정
        curHp = initHp;
    }

    private void OnCollisionEnter(Collision coll) {
        // 생명 수치가 0보다 크고 충돌체의 태그가 BULLET인 경우에 생명 수치를 차감
        if(curHp > 0 && coll.collider.CompareTag("BULLET"))
        {
            curHp -= 20;
            if(curHp <= 0)
            {
                StartCoroutine(PlayerDie());
            }
        }
    }

    private IEnumerator PlayerDie()
    {
        // Character Controller 컴포넌트 비활성화
        cc.enabled = false;
        // 리스폰 비활성화
        anim.SetBool(hashRespawn, false);
        // 캐릭터 사망 애니메이션 실행
        anim.SetTrigger(hashDie);

        yield return new WaitForSeconds(3.0f);

        // 리스폰 활성화
        anim.SetBool(hashRespawn, true);

        // 캐릭터 투명 처리
        SetPlayervisible(false);

        yield return new WaitForSeconds(1.5f);

        // 생성 위치를 재조정
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        // 리스폰 시 생명 초깃값 설정
        curHp = 100;
        // 캐릭터를 다시 보이게 처리
        SetPlayervisible(true);
        // CharacterController 컴포넌트 활성화
        cc.enabled = true;
    }

    // Renderer 컴포넌트를 활성/비활성화하는 함수 
    private void SetPlayervisible(bool isvisible)
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isvisible;
        }
    }
}
