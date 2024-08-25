using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneBehaviour : MonoBehaviour
{
	[SerializeField] private GameObject loadingBar;

	private RectTransform loadingBarRectTransform;
	private float loadingBarRectTransformWidth;

	// Start is called before the first frame update
	void Start()
    {
		loadingBarRectTransform = loadingBar.GetComponent<RectTransform>();
		loadingBarRectTransformWidth = loadingBarRectTransform.rect.size.x;

		LoadGameScene();
	}

	private void LoadGameScene()
    {
		StartCoroutine(AsyncLoadSceneCoroutune("Game"));
    }

	IEnumerator AsyncLoadSceneCoroutune(string sceneName)
	{
		var async_operation = SceneManager.LoadSceneAsync(sceneName);

		while (!async_operation.isDone)
		{
			loadingBarRectTransform.sizeDelta = new Vector2(async_operation.progress * loadingBarRectTransformWidth, loadingBarRectTransform.sizeDelta.y);
			yield return null;
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
	}
}
