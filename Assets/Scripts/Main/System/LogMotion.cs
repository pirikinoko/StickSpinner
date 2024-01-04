using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LogMotion : MonoBehaviour
{
    Image image;
    Text logText;
    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
        logText = transform.GetChild(0).gameObject.GetComponent<Text>();
        StartCoroutine(Motion());

        if (logText != null)
        {
            AdjustWidth();
        }
        else
        {
            Debug.LogError("Text component is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Motion()
    {
        // �t�F�[�h�C��
        yield return Fade(0f, 1f, 1f);

        // �ҋ@
        yield return new WaitForSeconds(3);

        // �t�F�[�h�A�E�g
        yield return Fade(1f, 0f, 1f); 

        // �I�u�W�F�N�g�̔j��
        Destroy(gameObject);
    }

    IEnumerator Fade(float startAlpha, float targetAlpha, float duration)
    {
        float startTime = Time.time;
        Color startColor = image.color;

        while (Time.time < startTime + duration)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        image.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    void AdjustWidth()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform logTxRectTransform = logText.GetComponent<RectTransform>();
        float scaleX = rectTransform.localScale.x;
        if (rectTransform != null)
        {
            // �������ɉ����ĉ����𒲐�
            float textScaleX = logTxRectTransform.localScale.x;
            float scaleDiff =  textScaleX / scaleX;
            float newFrameWidth = CalculateTextWidth() * scaleDiff / 3.8f;
            float newTextWidth = newFrameWidth * 33;
            rectTransform.sizeDelta = new Vector2(newFrameWidth, rectTransform.sizeDelta.y);
            logTxRectTransform.sizeDelta = new Vector2(newTextWidth, logTxRectTransform.sizeDelta.y);
        }
        else
        {
            Debug.LogError("RectTransform component is missing!");
        }
    }

    float CalculateTextWidth()
    {
        TextGenerator textGenerator = new TextGenerator();
        TextGenerationSettings generationSettings = logText.GetGenerationSettings(Vector2.zero);

        // �e�L�X�g�̕����v�Z
        float textWidth = textGenerator.GetPreferredWidth(logText.text, generationSettings);

        return textWidth;
    }
}
