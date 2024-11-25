using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ChatSizeFitter : MonoBehaviour
{
    [Header("Hiearchy")]
    [SerializeField] private RectTransform BubbleTransform;
    [SerializeField] private TextMeshProUGUI chatTxt;

    [Header("Padding")]
    [SerializeField] private float left = 50;
    [SerializeField] private float right = 40;
    [SerializeField] private float top = 10;
    [SerializeField] private float bottom = 20;

    [Header("Preferred Size")]
    [SerializeField] private float minTextWidth = 210;
    [SerializeField] private float maxTextWidth = 628;
    [SerializeField] private float mincontainerHeight = 70;

    public void InputChat(string text)
    {
        chatTxt.text = text;
        UpdateSpeechBubbleSize();
    }

    public void UpdateSpeechBubbleSize()
    {
        Vector2 chatTextSize = chatTxt.GetPreferredValues(chatTxt.text, maxTextWidth, Mathf.Infinity);

        float xPadding = left + right; 
        float yPadding = top + bottom;
        float fixedHeight = 35;

        float containerWidth = Mathf.Clamp(chatTextSize.x, minTextWidth, maxTextWidth);
        float containerHeight = Mathf.Max(chatTextSize.y + fixedHeight, mincontainerHeight);

        float bubbleWidth = containerWidth + xPadding;
        float bubbleHeight = containerHeight + yPadding;

        BubbleTransform.sizeDelta = new Vector2(bubbleWidth, bubbleHeight);
    }
}

[CustomEditor(typeof(ChatSizeFitter))]
public class ChatBubbleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ChatSizeFitter chatBubble = (ChatSizeFitter)target;

        if (GUILayout.Button("새로고침"))
        {
            chatBubble.UpdateSpeechBubbleSize();
        }
    }
}