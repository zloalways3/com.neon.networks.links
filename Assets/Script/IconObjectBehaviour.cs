using UnityEngine;
using UnityEngine.Events;

public class IconObjectBehaviour : MonoBehaviour
{
	[SerializeField] private bool _isLineComplete;

	public static UnityEvent OnSetLine = new UnityEvent();

	public void SetLineComplete(bool isLineComplete)
	{
		_isLineComplete = isLineComplete;
		OnSetLine.Invoke();
	}

	public bool IsLineComplete { get { return _isLineComplete; } }
}
