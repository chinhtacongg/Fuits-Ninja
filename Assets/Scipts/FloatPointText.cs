
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FloatPointText : MonoBehaviour
{
    private TextMeshPro floatPointText;

    private void Awake()
    {
        floatPointText = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        floatPointText.alpha = 2.0f;
        transform.localScale = Vector3.one;

        Sequence sq = DOTween.Sequence();

        sq.Append(transform.DOMoveY(transform.position.y + 1.5f, 0.5f)).
             Join(transform.DOScale(1.3f, 0.5f)).
             Join(floatPointText.DOFade(0, 0.5f)).OnComplete(() => gameObject.SetActive(false));
    }
  
}
