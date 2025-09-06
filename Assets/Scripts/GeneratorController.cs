using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Data;
using System.Collections;

public class GeneratorController : MonoBehaviour
{
    [SerializeField] private RectTransform group;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private GridLayoutGroup gridGroup;
    
    [SerializeField] UIManager uiManager;

    [SerializeField] GameObject explAnimPrefab;
    [SerializeField] private RectTransform fxPanelTransform;

    [SerializeField] private AudioClip popClip;

    public event Action OnGridFull;

    private readonly List<TMP_Text> cells = new();
    
    private int rows, cols;

    public int[] GetMatrix()
    {
        int n = cells.Count;
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = int.Parse(cells[i].text);
        return a;
    }

    public int Rows => rows;
    public int Cols => cols;

    private void OnEnable()
    {
        if (uiManager != null) uiManager.OnGenerate += Generate;
    }

    private void OnDisable()
    {
        if (uiManager != null) uiManager.OnGenerate -= Generate;
    }

    public void Clear()
    {
        foreach(Transform child in group) Destroy(child.gameObject);
        cells.Clear();
        rows = 0; 
        cols = 0;
    }
    
    public void Generate(int r, int c)
    {
        if (gridGroup) gridGroup.enabled = true;
        var fitter = group.GetComponent<ContentSizeFitter>();
        if (fitter) fitter.enabled = true;

        Clear();

        rows = r;
        cols = c;

        gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridGroup.constraintCount = cols;

        int total = rows * cols;

        StartCoroutine(GenerateFlow());
    }

    private IEnumerator GenerateFlow()
    {
        int total = rows * cols;

        for (int i = 0; i < total; i++)
        {
            GameObject num = Instantiate(numberPrefab, group);
            TMP_Text label = num.GetComponentInChildren<TMP_Text>();
            label.text = (i + 1).ToString();
            cells.Add(label);

            Highlight(i, true);
            yield return new WaitForSeconds(0.2f);
            Highlight(i, false);

            yield return new WaitForSeconds(0.05f);
        }

        OnGridFull?.Invoke();
        uiManager.ShowReadBtnPanel();
    }

    private void Highlight(int index, bool on)
    {
        if (index < 0 || index >= cells.Count) return;
        var txt = cells[index];
        txt.color = on ? Color.yellow : Color.white;
        
    }

    public void FreezeLayout()
    {
        var rectTrans = group;
        Vector2 size = rectTrans.sizeDelta;

        var fitter = rectTrans.GetComponent<ContentSizeFitter>();
        if (fitter) fitter.enabled = false;

        if (gridGroup) gridGroup.enabled = false;

        rectTrans.sizeDelta = size;
    }

    
    public void AnimateAndDestroy(int index, float duration = 0.15f)
    {
        if (index < 0 || index >= cells.Count) return;

        var text = cells[index];
        if (text == null) return;

        RectTransform blockTransform = (RectTransform)text.transform.parent;

        GameObject explosionInstance = Instantiate(explAnimPrefab, blockTransform.position, Quaternion.identity, fxPanelTransform);
        
        AudioManager.instance.PlaySfx(popClip);

        blockTransform.gameObject.SetActive(false);
    }
}
