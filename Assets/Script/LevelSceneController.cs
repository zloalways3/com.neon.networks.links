using TMPro;
using UnityEngine;

public class LevelSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		if (!PlayerPrefs.HasKey("PlayerLevel"))
		{
			PlayerPrefs.SetString("PlayerLevel", "Easy");
		}
	}

	public void SetLevel(TMP_Text text)
	{
		string level = text.text;
		PlayerPrefs.SetString("PlayerLevel", level);
	}
}
