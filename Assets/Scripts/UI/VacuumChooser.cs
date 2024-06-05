using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class VacuumChooser : MonoBehaviour
{

    private UIDocument _uiDocument;
    private ScrollView _scrollView;

    public PlayerModelPoolSO modelPool;
    public VisualTreeAsset modelListItem;

    public bool open = false;
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        _scrollView = _uiDocument.rootVisualElement.Q<ScrollView>("scroll_view");
    }

    private void Start()
    {
        if (_scrollView == null)
        {
            Debug.LogWarning("ScrollView is Null");
            return;
        }
        Refresh();
    }

    private void Refresh()
    {
        
        _scrollView.Clear();
        
        int indexOfCurrentModel = modelPool.playerModels.IndexOf(PlayerStateManager.Instance.currentModel);

        for (int i = 0; i < modelPool.playerModels.Count; i++)
        {
            int modelIndex = i;

            PlayerModelSO playerModelSo = modelPool.playerModels[modelIndex];

            VisualElement newModelListItem = modelListItem.CloneTree();

            VisualElement iconElement = newModelListItem.Q<VisualElement>("icon");
            Texture2D prefabModelPreview = AssetPreview.GetAssetPreview(playerModelSo.modelPrefab);
            iconElement.style.backgroundImage = new StyleBackground(prefabModelPreview);

            Label titleElement = newModelListItem.Q<Label>("title");
            titleElement.text = playerModelSo.modelName;


            bool current = indexOfCurrentModel == i;
            bool owned = PlayerStateManager.Instance.playerData.unlockedModels.Contains(modelIndex);

            PlayerStateManager.Instance.playerData.unlockedModels.ForEach(modelI => Debug.Log($"{modelI}"));
            
            if (current)
            {
                Debug.Log($"{playerModelSo.modelName} Current");
                newModelListItem.style.backgroundColor = new StyleColor(Color.cyan);
            } else if (!owned)
            {
                Debug.Log($"{playerModelSo.modelName} Not Owned");
                newModelListItem.style.opacity = 0.5f;
            } else
            {
                Debug.Log($"{playerModelSo.modelName} Owned");
                newModelListItem.RegisterCallback<ClickEvent>(evt =>
                {
                    PlayerStateManager.Instance.SetModel(modelIndex);
                    Close();
                    Refresh();
                });
            }

            _scrollView.Add(newModelListItem);
        }
    }

    private void Open()
    {
        open = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;   
    }

    private void Close()
    {
        open = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        _uiDocument.rootVisualElement.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!open)
                Open();
            else
                Close();
        }
    }
}
