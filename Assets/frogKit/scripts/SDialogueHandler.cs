using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SDialogueHandler : MonoBehaviour
{

    [Header("UI Components")]
    public Image background;
    public RawImage leftChar;
    public RawImage rightChar;
    public TextMeshProUGUI text;

    [Header("Data Settings")]
    public string jsonFileName = "dialogue";
    public string localizationTableName = "MyDialogueTable";
    public string nextSceneName;
    public float initialInputDelay = 5.0f;

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string charTexturePath;
        public string localizationKey;
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueLine> lines;
    }

    private DialogueData dialogueData;
    private int currentLineIndex = 0;
    private bool isWaitingForInput = false;
    private float inputAllowedTime = 3f;

    void Start()
    {
        text.text = "...";
        inputAllowedTime = Time.unscaledTime + initialInputDelay;
        StartCoroutine(InitializeAndStartDialogue());
    }

    private void OnNextPage(InputValue value)
    {
        // Only register on the true press event
        if (!value.isPressed) return;
        if (Time.unscaledTime < inputAllowedTime)
        {
            Debug.Log($"[Dialogue] Input blocked. Leftover interaction or spam protection active for another {inputAllowedTime - Time.unscaledTime:F2}s");
            return;
        }
        if (!isWaitingForInput) return;

        AdvanceDialogue();
    }


    void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile != null)
        {
            dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);
        }
        else
        {
            Debug.LogError($"Dialogue JSON file '{jsonFileName}' not found in Resources!");
        }
    }
    private IEnumerator InitializeAndStartDialogue()
    {
        yield return LocalizationSettings.InitializationOperation;
        var tableLoading = LocalizationSettings.StringDatabase.GetTableAsync(localizationTableName);
        yield return tableLoading;

        if (tableLoading.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
        {
            Debug.LogError($"[Localization] Total failure loading table '{localizationTableName}'. Does it actually exist in Addressables?");
            yield break;
        }

        // Now that everything is absolutely ready, load the data and display it
        LoadDialogueData();
        DisplayCurrentLine();

    }


    void DisplayCurrentLine()
    {
        text.text = "...";
        if (dialogueData == null || dialogueData.lines == null || currentLineIndex >= dialogueData.lines.Count) return;

        isWaitingForInput = false;
        DialogueLine line = dialogueData.lines[currentLineIndex];

        StartCoroutine(FetchLocalizedText(line.localizationKey));

        Texture2D charTexture = Resources.Load<Texture2D>(line.charTexturePath);

        if (line.speaker.ToLower() == "left")
        {
            UpdateCharacterUI(leftChar, charTexture, true);
            UpdateCharacterUI(rightChar, null, false);
        }
        else if (line.speaker.ToLower() == "right")
        {
            UpdateCharacterUI(rightChar, charTexture, true);
            UpdateCharacterUI(leftChar, null, false);
        }
    }

    private IEnumerator FetchLocalizedText(string key)
    {
        text.text = "...";

        var loadingOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizationTableName, key);

        yield return loadingOperation;

        if (loadingOperation.IsDone && !string.IsNullOrEmpty(loadingOperation.Result))
        {
            text.text = loadingOperation.Result;
        }
        else
        {
            text.text = $"[Missing string for: {key}]";
            Debug.LogWarning($"Could not find key '{key}' in table '{localizationTableName}'");
        }


        isWaitingForInput = true;
    }

    void UpdateCharacterUI(RawImage rawImage, Texture2D texture, bool isSpeaking)
    {
        if (rawImage == null) return;
        if (texture != null)
        {
            rawImage.texture = texture;
            rawImage.enabled = true;
            rawImage.color = isSpeaking ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            rawImage.enabled = false;
        }
    }

    public void AdvanceDialogue()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueData.lines.Count)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        text.text = "";
        gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Dialogue finished. Loading scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Dialogue finished, but no Next Scene Name was assigned in the inspector.");
            gameObject.SetActive(false);
        }
    }

}
