using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class hart : MonoBehaviour
{
    [SerializeField]
    int count = 5;

    [SerializeField]
    Sprite image;

    // OnValidate の遅延呼び出しが重複登録されないようにするフラグ
    bool validateScheduled = false;

    void OnValidate()
    {
        if (image == null)
            return;

#if UNITY_EDITOR
        if (validateScheduled)
            return;

        validateScheduled = true;
        EditorApplication.delayCall += OnValidateDelay;
#endif
    }

#if UNITY_EDITOR
    void OnValidateDelay()
    {
        // 実行されたら自身を解除してフラグを戻す
        EditorApplication.delayCall -= OnValidateDelay;
        validateScheduled = false;

        // オブジェクトが既に破棄されていれば何もしない
        if (this == null)
            return;

        UpdateHart();
    }
#endif

    /// <summary>
    /// 子要素のハートをすべて削除する
    /// </summary>
    void DestoryAllHart()
    {
        List<GameObject> array = new();

        foreach (Transform child in transform)
        {
            array.Add(child.gameObject);
        }

        for (int cnt = 0; cnt < array.Count; cnt++)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(array[cnt]);
            else
                Destroy(array[cnt]);
#else
            Destroy(array[cnt]);
#endif
        }
    }

    /// <summary>
    /// 規定数のハートを作成する
    /// </summary>
    void CreateHart()
    {
        for (int cnt = transform.childCount; cnt < count; cnt++)
        {
            GameObject obj = new GameObject(nameof(hart));
            obj.transform.parent = transform;
            Image image = obj.AddComponent<Image>();
            image.sprite = this.image;

            Vector3 pos = new Vector3(cnt * 100, 0, 0);
            obj.transform.localPosition = pos;
        }
    }

    void UpdateHart()
    {
        CreateHart();

        if (transform.childCount != count)
            DestoryAllHart();
    }

    void Update()
    {
        UpdateHart();

        if (Input.GetKeyDown(KeyCode.B))
        {
            count++;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            count--;
        }
    }
}
