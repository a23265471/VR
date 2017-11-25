using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using UnityEngine.UI;//namespace 命名空間
public class ShootingGalleryController : MonoBehaviour//class 屬性
{
    public UIController uiController;//Field 欄位
    public Reticle reticle;
    public SelectionRadial selectionRadial;
    public SelectionSlider selectionSlider;

    public Image timerBar;
    public float gameDuration = 30f;
    public float endDelay=1.5f;

    public Collider spawnCollider;
    public ObjectPool targetObectPool;
    public float spawnProbabilty = 0.7f;
    public float spawnTnterval = 1f;
    public bool IsPlaying//property 屬性
    {
        private set;
        get;

    }

    private IEnumerator Start()//Method 方法
    {
        SessionData.SetGameType(SessionData.GameType.SHOOTER180);
        while(true)
        {
            
            yield return StartCoroutine(StartPhase());

            yield return StartCoroutine(PlayerPhase());

            
            yield return StartCoroutine(EndPhase());
        }

    }

    private IEnumerator StartPhase()
    {
        yield return StartCoroutine(uiController.ShowIntroUI());
        reticle.Show();
        selectionRadial.Hide();
        yield return StartCoroutine(selectionSlider.WaitForBarToFill());
        yield return StartCoroutine(uiController.HideIntroUI());

    }
    private IEnumerator PlayerPhase()
    {
        yield return StartCoroutine(uiController.ShowPlayerUI());
        IsPlaying = true;
        reticle.Show();
        SessionData.Restart();
        float gameTimer = gameDuration;
        float spawnTimer = 0f;
        while (gameTimer > 0f)
        {
            if (spawnTimer <= 0f)
            {
                if (Random.value < spawnProbabilty)
                {
                    spawnTimer = spawnTnterval;
                    Spawn();

                }


            }
            yield return null;
            gameTimer -= Time.deltaTime;
            spawnTimer -= Time.deltaTime;
            timerBar.fillAmount = gameTimer / gameDuration;
        }

        IsPlaying = false;
        yield return StartCoroutine(uiController.HidePlayerUI());
    }

    private void Spawn()
    {
        GameObject target = targetObectPool.GetGameObjectFromPool();
        target.transform.position = SpawnPosition();
    }

    private Vector3 SpawnPosition()
    {
        Vector3 center = spawnCollider.bounds.center;
        Vector3 extents = spawnCollider.bounds.extents;
        float x = Random.Range(center.x-extents.x,center.x+extents.x);
        float y = Random.Range(center.y - extents.y, center.y + extents.y);
        float z = Random.Range(center.z - extents.z, center.z + extents.z);
        return new Vector3(x, y, z);

    }

    private IEnumerator EndPhase()
    {
        yield return StartCoroutine(uiController.ShowOutroUI());
        yield return new WaitForSeconds(endDelay);
        yield return StartCoroutine(selectionRadial.WaitForSelectionRadialToFill());
        yield return StartCoroutine(uiController.HideOutroUI());
    }
}
