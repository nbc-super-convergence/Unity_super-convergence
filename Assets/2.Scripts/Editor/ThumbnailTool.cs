using UnityEditor;
using UnityEngine;
using System.IO;

public class ThumbnailTool : EditorWindow
{
    private string prefabPath = "Camera/Blue"; // Resources 폴더 하위 경로
    private RenderTexture renderTexture;
    private Camera thumbnailCamera;
    private GameObject tempInstance;

    private float cameraAngleY = 0; // 카메라의 Y축 회전 각도
    private float fieldOfView = 60; // 카메라 FOV (Field of View)

    [MenuItem("Tools/Prefab Thumbnail Creator")]
    public static void ShowWindow()
    {
        GetWindow<ThumbnailTool>("Prefab Thumbnail Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Thumbnail Creator", EditorStyles.boldLabel);

        // 프리팹 경로 입력
        prefabPath = EditorGUILayout.TextField("Prefab Path (Resources):", prefabPath);

        // 카메라 시야 각도 입력
        cameraAngleY = EditorGUILayout.FloatField("Camera Y Rotation (Angle):", cameraAngleY);

        // FOV 입력
        fieldOfView = EditorGUILayout.FloatField("Field of View (FOV):", fieldOfView);

        // 썸네일 생성 및 미리보기
        if (GUILayout.Button("Preview Thumbnail"))
        {
            GenerateThumbnail(preview: true);
        }

        // 저장 버튼
        if (GUILayout.Button("Save Thumbnail"))
        {
            GenerateThumbnail(preview: false);
        }

        GUILayout.Space(10);

        // 미리보기 RenderTexture 출력
        if (renderTexture != null)
        {
            GUILayout.Label("Preview:");
            GUILayout.Box(renderTexture, GUILayout.Width(256), GUILayout.Height(256));
        }
    }

    private void GenerateThumbnail(bool preview)
    {
        // 기존 임시 인스턴스 제거
        if (tempInstance != null)
        {
            DestroyImmediate(tempInstance);
        }

        // 프리팹 로드
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
            return;
        }

        // 프리팹 인스턴스 생성
        tempInstance = Instantiate(prefab);
        tempInstance.transform.position = Vector3.zero;

        // 카메라 생성
        if (thumbnailCamera == null)
        {
            GameObject cameraObj = new GameObject("ThumbnailCamera");
            thumbnailCamera = cameraObj.AddComponent<Camera>();
            thumbnailCamera.clearFlags = CameraClearFlags.SolidColor;
            thumbnailCamera.backgroundColor = new Color(0, 0, 0, 0); // 투명 배경
        }

        // RenderTexture 생성
        if (renderTexture == null || renderTexture.width != 1024)
        {
            renderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
        }

        thumbnailCamera.targetTexture = renderTexture;

        // 오브젝트 중심과 크기 계산
        Renderer renderer = tempInstance.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("The prefab does not have a Renderer component.");
            return;
        }

        Bounds bounds = renderer.bounds;

        // 카메라 위치 설정 (Y축 각도 조정)
        float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        Vector3 cameraPosition = bounds.center + Quaternion.Euler(0, cameraAngleY, 0) * new Vector3(0, 0, -maxExtent * 2);
        thumbnailCamera.transform.position = cameraPosition;

        // 카메라가 오브젝트 중심을 바라보도록 설정
        thumbnailCamera.transform.LookAt(bounds.center);

        // FOV 설정
        thumbnailCamera.fieldOfView = fieldOfView;

        // 직교 카메라로 전환 가능 (선택적으로 추가)
        // thumbnailCamera.orthographic = true;

        // 캡처 실행
        RenderTexture.active = renderTexture;
        thumbnailCamera.Render();

        if (!preview)
        {
            // 저장 경로 설정
            string folderPath = Application.dataPath + "/3.Downloads/0.Thumbnail";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = $"{folderPath}/{prefab.name}_Thumbnail.png";

            // Texture2D로 저장
            Texture2D thumbnail = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            thumbnail.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            thumbnail.Apply();

            byte[] bytes = thumbnail.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"Thumbnail saved at: {filePath}");

            // Texture 메모리 해제
            DestroyImmediate(thumbnail);
        }

        RenderTexture.active = null;

        // 임시 객체 삭제
        DestroyImmediate(tempInstance);
    }
}
