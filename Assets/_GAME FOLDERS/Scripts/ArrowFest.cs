using UnityEngine;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using System.Collections;

public class ArrowFest : MonoBehaviour
{
    private Camera _camera;
    private bool isDecrease = false;

    [SerializeField] private GameObject arrowObject;
    [SerializeField] private Transform parent;
    public List<GameObject> arrows = new List<GameObject>();
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private float minX, maxX;


    [SerializeField] private float distance;
    [Range(0, 300)] [SerializeField] private int arrowCount = 10;
    private void Start()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            GetRay();
        }
    }
    private void OnValidate()
    {
        if (arrowCount > arrows.Count && !isDecrease)
        {
            CreateArrow();
        }
        else if (arrows.Count > arrowCount)
        {
            isDecrease = true;
            DestroyArrows();
        }
        else
            Align();
    }

    private void CreateArrow()
    {
        for (int i = arrows.Count; i < arrowCount; i++) // ex : arrows.Count : 20  arrowCount : 40
        {
            GameObject arrow = Instantiate(arrowObject, parent);
            arrows.Add(arrow);
            arrow.transform.localPosition = Vector3.zero;
        }
        Align();
    }
    IEnumerator DestroyObject(GameObject gameObject)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(gameObject);
    }
    private void DestroyArrows()
    {
        for (int i = arrows.Count - 1; i > arrowCount; i--)
        {
            GameObject arrow = arrows[arrows.Count - 1];
            arrows.RemoveAt(arrows.Count - 1);
            EditorCoroutineUtility.StartCoroutine(DestroyObject(arrow), this);
        }
        isDecrease = false;
        Align();
    }

    private void GetRay()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.transform.position.z;

        Ray ray = _camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 250, layerMask))
        {
            Vector3 mouse = hit.point;
            mouse.x = Mathf.Clamp(mouse.x, minX, maxX);

            distance = mouse.x;
            Align();
        }
    }

    private void Align()
    {
        float angle = 1f;
        float arrowCount = arrows.Count;
        angle = 360 / arrowCount;

        for (int i = 0; i < arrowCount; i++)
        {
            MoveObjects(arrows[i].transform, i * angle);
        }
    }
    private void MoveObjects(Transform objectTransform, float degree)
    {
        Vector3 pos = Vector3.zero;
        pos.x = Mathf.Cos(degree * Mathf.Deg2Rad);
        pos.y = Mathf.Sin(degree * Mathf.Deg2Rad);

        objectTransform.localPosition = pos * distance;
    }

}
