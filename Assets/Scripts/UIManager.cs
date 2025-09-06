using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField textRows;
    [SerializeField] private TMP_InputField textColumns;
    
    [SerializeField] private GameObject warningPanel;
    [SerializeField] protected TextMeshProUGUI warningText;

    [SerializeField] private int min = 2;
    [SerializeField] private int max = 20;

    [SerializeField] private GameObject pauseMusic;
    [SerializeField] private GameObject resumeMusic;

    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip music;

    [SerializeField] private GameObject readBtnPanel;
    [SerializeField] private GameObject readPanel;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private MatrixReader matrixReader;
    [SerializeField] private GeneratorController generatorController;

    private int rows = 0;
    private int cols = 0;

    public event Action<int, int> OnGenerate;
    public event Action OnReadMatrix;

    private void OnEnable()
    {
        matrixReader.OnReadComplete += ManageReadEnd;
    }

    private void OnDisable()
    {
        matrixReader.OnReadComplete -= ManageReadEnd;
    }

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void Reset()
    {
        if (textRows != null) textRows.text = "";
        if (textColumns != null) textColumns.text = "";
    }

    public void Generate()
    {
        AudioManager.instance.PlaySfx(click);
        if (textRows == null || textColumns == null)
        {
            StartCoroutine(Warning("*Missing input fields."));
            return;
        }

        string rowsText = textRows.text?.Trim();
        string colsText = textColumns.text?.Trim();

        if (string.IsNullOrEmpty(rowsText) || string.IsNullOrEmpty(colsText))
        {
            StartCoroutine(Warning("*Please enter rows and columns."));
            return;
        }

        int r, c;

        if (!int.TryParse(rowsText, out r) || !int.TryParse(colsText, out c))
        {
            StartCoroutine(Warning("*Please enter valid numbers."));
            return;
        }

        if ((r <= min || c <= min) || (r >= max || c >= max))
        {
            StartCoroutine(Warning($"*Numbers must be between {min} and {max}."));
            return;
        }

        rows = r;
        cols = c;

        OnGenerate?.Invoke(rows, cols);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private IEnumerator Warning(string message)
    {
        warningPanel.SetActive(true);
        warningText.text = message;
        yield return new WaitForSeconds(3);
        warningPanel.SetActive(false);
    }

    public void MusicPause()
    {
        if (pauseMusic.activeSelf == true)
        {
            AudioManager.instance.PauseMusic();
            pauseMusic.SetActive(false);
            resumeMusic.SetActive(true);
        }
        else
        {
            AudioManager.instance.ResumeMusic();
            pauseMusic.SetActive(true);
            resumeMusic.SetActive(false);
        } 
    }

    public void ShowReadBtnPanel()
    {
        if(readBtnPanel.activeSelf==false) readBtnPanel.SetActive(true);
        else readBtnPanel.SetActive(false);
    }

    public void ShowReadPanel() 
    {
        if (readPanel == null) return;
        else readPanel.SetActive(true);

        ShowReadBtnPanel();
        OnReadMatrix?.Invoke();
    }

    public void HideReadPanel()
    {
        readPanel.SetActive(true);
    }

    public void ManageReadEnd()
    {
        AudioManager.instance.PlaySfx(success);
        
        ShowSuccessPanel(true);
    }

    private void ShowSuccessPanel(bool state)
    {
        if (successPanel == null) return;
        successPanel.SetActive(state);
    }

    public void NewJob()
    {
        AudioManager.instance.PlayUI(click);
        ShowSuccessPanel(false);
        if(readPanel.activeSelf == true) readPanel.SetActive(false);
        generatorController.Clear();
        settingsPanel.SetActive(true);
        textRows.text = string.Empty;
        textColumns.text = string.Empty;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
