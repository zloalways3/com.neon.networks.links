using UnityEngine;

public class SpriteScreenFitter : MonoBehaviour
{
	public Vector2 anchorPercentage; // ������� �� ������ (0-1) ��� �����
	public Vector2 sizeInUnits; // ������ � ������, ������� ����� �������������� ��� ���������������

	void Start()
	{
		UpdatePositionAndScale();
	}

	void UpdatePositionAndScale()
	{
		Vector2 screenSize = new Vector2(Screen.width, Screen.height);
		Vector2 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenSize.x * anchorPercentage.x, screenSize.y * anchorPercentage.y, 0));
		transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);

		float aspectRatio = screenSize.x / screenSize.y;
		transform.localScale = new Vector3(sizeInUnits.y, sizeInUnits.x / aspectRatio, 1);
	}

	void Update()
	{

		UpdatePositionAndScale();
		
	}
}