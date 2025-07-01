using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button), typeof(Image))]
public class LevelButton : MonoBehaviour
{
    LevelInfo levelInfo;
    int levelDataIdx;
    Button button;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image clearedImg;
    [SerializeField] Image lockedImg;

    public void Init(LevelInfo levelInfo, int levelDataIdx, bool isLocked)
    {
        this.levelInfo = levelInfo;
        this.levelDataIdx = levelDataIdx;
        nameText.text = levelInfo.levelName;
        clearedImg.gameObject.SetActive(GameDataSystemManager.Instance.IsLevelCleared(levelInfo.levelName));
        lockedImg.gameObject.SetActive(isLocked);
        GetComponent<Image>().sprite = levelInfo.thumbnail;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnLevelButtonClicked);
        button.enabled = !isLocked;
    }

    private void OnLevelButtonClicked()
    {
        // LevelManager.Instance.SetCurrentWaveNum(levelDataIdx);
        SceneSystemManager.Instance.ChangeScene(levelInfo.sceneName);
    }
}
