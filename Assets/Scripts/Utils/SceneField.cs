using UnityEngine;

[System.Serializable]
public class SceneField
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";

    public string SceneName
    {
        get { return sceneName; }
    }

    // implicit conversion to string
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.SceneName;
    }
}