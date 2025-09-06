using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MatrixReader : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GeneratorController generatorController;
    [SerializeField] private TextMeshProUGUI numbersTxt;
    
    int rows = 0, cols = 0;
    int[] matrix;

    public event System.Action OnReadComplete;

    private void OnEnable()
    {
        if (uiManager != null) uiManager.OnReadMatrix += ReadMatrix;
    }

    private void OnDisable()
    {
        if (uiManager != null) uiManager.OnReadMatrix -= ReadMatrix;
    }

    private void ReadMatrix()
    {
        if (generatorController == null || numbersTxt == null) return;

        matrix = generatorController.GetMatrix();
        rows = generatorController.Rows;
        cols = generatorController.Cols;

        generatorController.FreezeLayout();
        StartCoroutine(WriteNumbers());
    }

    private IEnumerator WriteNumbers()
    {
        string seq = traversa(matrix, rows, cols); 
        string[] parts = seq.Split(',');

        numbersTxt.text = "";

        for (int i = 0; i < parts.Length; i++)
        {
            float showUp = RandomNum(0.1f, 1f);
            if (i > 0) numbersTxt.text += ",";
            numbersTxt.text += parts[i];

            if (int.TryParse(parts[i], out int v))
                generatorController.AnimateAndDestroy(v - 1, showUp);

            yield return new WaitForSeconds(showUp);
        }
        StartCoroutine(NowWait(2f));
    }

    private IEnumerator NowWait(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnReadComplete?.Invoke();
    }

    private float RandomNum(float a, float b)
    {
        return UnityEngine.Random.Range(a, b);
    }

    private string traversa(int[] matrix, int rows, int columns)
    {
        var list = new List<int>();

        int top = 0;
        int bottom = rows - 1;
        int left = 0;
        int right = columns - 1;

        while (top <= bottom && left <= right) 
        {
            for (int column = left; column <= right; column++)
                list.Add(matrix[top * columns + column]);
            top++;

            for (int row = top; row <= bottom && left <= right; row++)
                list.Add(matrix[row * columns + right]);
            right--;

            for (int column = right; column >= left && top <= bottom; column--)
                list.Add(matrix[bottom * columns + column]);
            bottom--;

            for (int row = bottom; row >= top && left <= right; row--)
                list.Add(matrix[row * columns + left]);
            left++;
        }

        return string.Join(",", list);
    }
}
