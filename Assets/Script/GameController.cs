using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] bool isGameOver;

	[SerializeField] private float timer;

	private List<IconObjectBehaviour> iconObjects = new List<IconObjectBehaviour>();

    public static UnityEvent OnGameOver = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerTime"))
        {
            PlayerPrefs.SetFloat("PlayerTime", 0);
        }

        IconObjectBehaviour.OnSetLine.AddListener(CheckIfGameOver);
		BoardBehaviour.OnBoardFilled.AddListener(IconObjectsInstantiate);

        timer = 0;

        levelText.text = PlayerPrefs.GetString("PlayerLevel");
	}

	private void FixedUpdate()
	{
		if(!isGameOver) 
        {
            timer += Time.deltaTime;
            SetTimer();
		}
	}

    private void SetTimer()
    {
		float minutes = Mathf.FloorToInt(timer / 60);
		float seconds = Mathf.FloorToInt(timer % 60);

		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	private void IconObjectsInstantiate()
    {
		iconObjects = FindObjectsOfType<IconObjectBehaviour>().ToList();
	}

	private void CheckIfGameOver()
    {
        isGameOver = true;
        foreach (IconObjectBehaviour iconObject in iconObjects)
        {
            if (!iconObject.IsLineComplete)
            {
                isGameOver = false; break;
			}
		}

        if (isGameOver)
        {
            SceneManager.LoadScene("Win");
			PlayerPrefs.SetFloat("PlayerTime", timer);
			OnGameOver.Invoke();
		}
    }
}
