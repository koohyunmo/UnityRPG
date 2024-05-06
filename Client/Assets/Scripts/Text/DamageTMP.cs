using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTMP : MonoBehaviour
{
    [SerializeField]TextMeshPro damageText;
    private float floatSpeed = 2.0f; // 텍스트가 올라가는 속도
    private float fadeDuration = 1.0f; // 텍스트가 사라지는 데 걸리는 시간
    private Color originColor = new Color();

    private void Start() 
    {
        originColor = damageText.color;
    }

    public void Spawn(int damage)
    {
        damageText.text = "-"+damage.ToString();
        originColor = damageText.color;

        StopCoroutine(co_TextAnim());
        StartCoroutine(co_TextAnim());
    }

    IEnumerator co_TextAnim()
    {
        float elapsedTime = 0;
        Vector3 startPosition = damageText.transform.position;
        Color startColor = damageText.color;
        ;
        while (fadeDuration > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            float fadeAmount = elapsedTime / fadeDuration;

            // 텍스트 색상 투명도 조절
            Color currentColor = startColor;
            currentColor.a = Mathf.Lerp(startColor.a, 0, fadeAmount);
            damageText.color = currentColor;

            // 텍스트 위치 조절
            Vector3 newPosition = startPosition + new Vector3(0, floatSpeed * elapsedTime, 0);
            damageText.transform.localPosition = newPosition;

            yield return null;
        }

        Managers.Resource.Destroy(gameObject);
    }

}
