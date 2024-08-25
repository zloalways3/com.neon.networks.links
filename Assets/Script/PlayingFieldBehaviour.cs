using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefub;

	public Vector2 AreaSize = new Vector2 (5, 5);
    public Grid grid;

    private List<GameObject> cells = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
		string Level = PlayerPrefs.GetString("PlayerLevel");

        switch (Level)
        {
            case "EASY": AreaSize = Vector2.one * 5; break;
            case "MIDDLE": AreaSize = Vector2.one * 6; break;
            case "HARD": AreaSize = Vector2.one * 7; break;
        }

		grid = GetComponent<Grid>();
        SetGridobject();
		CreateField();
	}

    private void CreateField()
    {
        for(int i = 0; i < AreaSize.x; i++)
        {
            for (int j = 0; j < AreaSize.y; j++)
            {
                var cell = Instantiate(cellPrefub, transform);
                cells.Add(cell);

                Vector3 pos = grid.GetCellCenterLocal(new Vector3Int(i, j, 0));
				cell.transform.localPosition = pos;
                cell.transform.localScale = grid.cellSize * 0.4f;
			}
        }
    }

    private void SetGridobject()
    {
        transform.localScale = Vector3.one * 5f / AreaSize;
    }
}
