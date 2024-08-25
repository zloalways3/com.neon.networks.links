using TMPro;
using UnityEngine;

public class WinSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text LevelText;

    // Start is called before the first frame update
    void Start()
    {
        timer.text = PlayerPrefs.GetFloat("PlayerTime").ToString("00:00");
        LevelText.text = PlayerPrefs.GetString("PlayerLevel").ToUpper() + " LEVEL\nCOMPLETE";
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
