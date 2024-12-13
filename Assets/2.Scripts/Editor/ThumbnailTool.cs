using UnityEditor;
using UnityEngine;
using System.IO;

public class ThumbnailTool : EditorWindow
{
    private string prefabPath = "Camera/Blue"; // Resources ���� ���� ���
    private RenderTexture renderTexture;
    private Camera thumbnailCamera;
    private GameObject tempInstance;

    private float cameraAngleY = 0; // ī�޶��� Y�� ȸ�� ����
    private float fieldOfView = 60; // ī�޶� FOV (Field of View)

    [MenuItem("Tools/Prefab Thumbnail Creator")]
    public static void ShowWindow()
    {
        GetWindow<ThumbnailTool>("Prefab Thumbnail Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Thumbnail Creator", EditorStyles.boldLabel);

        // ������ ��� �Է�
        prefabPath = EditorGUILayout.TextField("Prefab Path (Resources):", prefabPath);

        // ī�޶� �þ� ���� �Է�
        cameraAngleY = EditorGUILayout.FloatField("Camera Y Rotation (Angle):", cameraAngleY);

        // FOV �Է�
        fieldOfView = EditorGUILayout.FloatField("Field of View (FOV):", fieldOfView);

        // ����� ���� �� �̸�����
        if (GUILayout.Button("Preview Thumbnail"))
        {
            GenerateThumbnail(preview: true);
        }

        // ���� ��ư
        if (GUILayout.Button("Save Thumbnail"))
        {
            GenerateThumbnail(preview: false);
        }

        GUILayout.Space(10);

        // �̸����� RenderTexture ���
        if (renderTexture != null)
        {
            GUILayout.Label("Preview:");
            GUILayout.Box(renderTexture, GUILayout.Width(256), GUILayout.Height(256));
        }
    }

    private void GenerateThumbnail(bool preview)
    {
        // ���� �ӽ� �ν��Ͻ� ����
        if (tempInstance != null)
        {
            DestroyImmediate(tempInstance);
        }

        // ������ �ε�
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
            return;
        }

        // ������ �ν��Ͻ� ����
        tempInstance = Instantiate(prefab);
        tempInstance.transform.position = Vector3.zero;

        // ī�޶� ����
        if (thumbnailCamera == null)
        {
            GameObject cameraObj = new GameObject("ThumbnailCamera");
            thumbnailCamera = cameraObj.AddComponent<Camera>();
            thumbnailCamera.clearFlags = CameraClearFlags.SolidColor;
            thumbnailCamera.backgroundColor = new Color(0, 0, 0, 0); // ���� ���
        }

        // RenderTexture ����
        if (renderTexture == null || renderTexture.width != 1024)
        {
            renderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
        }

        thumbnailCamera.targetTexture = renderTexture;

        // ������Ʈ �߽ɰ� ũ�� ���
        Renderer renderer = tempInstance.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("The prefab does not have a Renderer component.");
            return;
        }

        Bounds bounds = renderer.bounds;

        // ī�޶� ��ġ ���� (Y�� ���� ����)
        float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        Vector3 cameraPosition = bounds.center + Quaternion.Euler(0, cameraAngleY, 0) * new Vector3(0, 0, -maxExtent * 2);
        thumbnailCamera.transform.position = cameraPosition;

        // ī�޶� ������Ʈ �߽��� �ٶ󺸵��� ����
        thumbnailCamera.transform.LookAt(bounds.center);

        // FOV ����
        thumbnailCamera.fieldOfView = fieldOfView;

        // ���� ī�޶�� ��ȯ ���� (���������� �߰�)
        // thumbnailCamera.orthographic = true;

        // ĸó ����
        RenderTexture.active = renderTexture;
        thumbnailCamera.Render();

        if (!preview)
        {
            // ���� ��� ����
            string folderPath = Application.dataPath + "/3.Downloads/0.Thumbnail";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = $"{folderPath}/{prefab.name}_Thumbnail.png";

            // Texture2D�� ����
            Texture2D thumbnail = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            thumbnail.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            thumbnail.Apply();

            byte[] bytes = thumbnail.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"Thumbnail saved at: {filePath}");

            // Texture �޸� ����
            DestroyImmediate(thumbnail);
        }

        RenderTexture.active = null;

        // �ӽ� ��ü ����
        DestroyImmediate(tempInstance);
    }
}
