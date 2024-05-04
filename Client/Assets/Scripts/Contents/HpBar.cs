using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private GameObject hpBar = null;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio,0,1);
        hpBar.transform.localScale = new Vector3(ratio, 1,1);
    }
}
