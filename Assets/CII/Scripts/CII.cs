using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CII : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] Vector2Int gridCount = new Vector2Int(20, 20); // グリッドの個数 (W, H)
    [SerializeField] Vector2 gridSize = new Vector2(4.8f, 4.8f); // グリッドのサイズ (W, H) [mm]
    [SerializeField] Vector2 displaySize = new Vector2(277.0f, 156.0f); // ディスプレイのサイズ (W, H) [mm]
    [SerializeField] float lensToEyeDist = 600.0f; // レンズアレイから眼までの距離 [mm]
    [SerializeField] float displayToLensDist = 1.4f; // ディスプレイからレンズまでの距離 [mm]

    float ratioOfD2lToD2e; // display to lens distance と display to eye distance の比

    [Header("Camera")]
    [SerializeField] Camera cameraPrefab;
    [SerializeField] float cameraPitch = 0.043f; // 隣接するカメラ間の距離


    float cameraFov;
    List<List<Camera>> cameraList = new List<List<Camera>>(); // カメラの二次元リスト

    Vector2 viewportSize, viewportOffset; // ビューポートのサイズ、位置のオフセット

    bool shouldZoom = false;
    bool shouldArrangeCamera = false;
    
    void Start()
    {
        ratioOfD2lToD2e = displayToLensDist / (displayToLensDist + lensToEyeDist);

        Vector2 elementalImageSize = (1.0f + displayToLensDist / lensToEyeDist) * gridSize;
        viewportSize = elementalImageSize / displaySize;
        viewportOffset = (Vector2.one - gridCount * viewportSize) / 2.0f;

        cameraFov = cameraPrefab.fieldOfView;

        // カメラアレイの生成
        for (int x = 0; x < gridCount.x; x++)
        {
            List<Camera> col = new List<Camera>();
            for(int y = 0; y < gridCount.y; y++)
            {
                Camera cam = Instantiate(cameraPrefab, this.transform);
                cam.transform.localPosition = CalcCameraPosition(x, y);
                cam.lensShift = new Vector2(x - gridCount.x / 2, y - gridCount.y / 2) * ratioOfD2lToD2e;
                cam.rect = new Rect(x * viewportSize.x + viewportOffset.x, y * viewportSize.y + viewportOffset.y, viewportSize.x, viewportSize.y);
                col.Add(cam);
            }
            cameraList.Add(col);
        }
    }

    
    void Update()
    {
        // ズーム
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0.0f)
        {
            cameraFov = Mathf.Clamp(cameraFov - scrollInput * 50.0f * Time.deltaTime, 0.1f, 180.0f);
            shouldZoom = true;
        }

        // パラメータの変更
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraPitch = Mathf.Clamp(cameraPitch + 0.001f, 0.001f, 3.0f);
            shouldArrangeCamera = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraPitch = Mathf.Clamp(cameraPitch - 0.001f, 0.001f, 3.0f);
            shouldArrangeCamera = true;
        }
        
        for(int x = 0; x < gridCount.x; x++)
        {
            for(int y = 0; y < gridCount.y; y++)
            {
                // ズーム
                if (shouldZoom)
                {
                    cameraList[x][y].fieldOfView = cameraFov;
                }
                // カメラの再配置
                if (shouldArrangeCamera)
                {
                    cameraList[x][y].transform.localPosition = CalcCameraPosition(x, y);
                }

                // フラスタムの設定
                cameraList[x][y].lensShift = new Vector2(x - gridCount.x / 2, y - gridCount.y / 2) * ratioOfD2lToD2e;
                // ビューポートの設定
                cameraList[x][y].rect = new Rect(x * viewportSize.x + viewportOffset.x, y * viewportSize.y + viewportOffset.y, viewportSize.x, viewportSize.y);
            }
        }

        shouldZoom = false;
        shouldArrangeCamera = false;
    }

    // カメラ位置を計算
    Vector3 CalcCameraPosition(int x, int y)
    {
        float camPosX = (x - gridCount.x / 2) * cameraPitch;
        float camPosY = (y - gridCount.y / 2) * cameraPitch;
        Vector3 camPos = new Vector3(camPosX, camPosY, 0.0f);
        return camPos;
    }
}
