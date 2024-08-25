using UnityEngine;

public class SoundBehaviour : MonoBehaviour
{
	[SerializeField] private AudioClip _LineDrawing;
	[SerializeField] private AudioClip _SetLine;
	[SerializeField] private AudioSource _music;
	[SerializeField] private AudioSource _btnSound;
	private AudioSource _soundSource;

	private string _isMusicPlayingKey = "_isMusicPlaying";
	private string _isSoundPlayingKey = "_isSoundPlaying";

	private bool _isMusicPlaying;
	private bool _isSoundPlaying;

	// Start is called before the first frame update
	void Start()
	{
		ButtonBehaviour.onMusicStateChange.AddListener(ChangeMusicPlayState);
		ButtonBehaviour.onPlayButtonSound.AddListener(PlayButtonSound);
		CellBehaviour.OnLineDrawing.AddListener(PlayOnLineDrawingSound);
		IconObjectBehaviour.OnSetLine.AddListener(PlayOnSetLineSound);

		ButtonBehaviour.onPlayAudioClipSound.AddListener(PlaySound);

		_isMusicPlaying = PlayerPrefs.GetInt(_isMusicPlayingKey) == 1;

		if (_music != null && _isMusicPlaying)
		{
			_music.Play();
		}

		_soundSource = gameObject.AddComponent<AudioSource>();
	}

	public void ChangeMusicPlayState()
	{
		if (_music == null)
			return;

		_isMusicPlaying = PlayerPrefs.GetInt(_isMusicPlayingKey) == 1;
		if (_isMusicPlaying)
		{
			_music.Play();
		}
		else
		{
			_music.Stop();
		}
	}

	public void PlaySound(AudioClip sound)
	{
		_soundSource.clip = sound;
		_soundSource.Play();
	}

	public void PlayButtonSound()
	{
		if(_btnSound != null)
			_btnSound.Play();
	}

	public void PlayOnLineDrawingSound()
	{
		_soundSource.clip = _LineDrawing;
		_soundSource.Play();
	}

	public void PlayOnSetLineSound()
	{
		_soundSource.clip = _SetLine;
		_soundSource.Play();
	}
}
