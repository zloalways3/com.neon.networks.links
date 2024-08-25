using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BoardBehaviour : MonoBehaviour
{
	[SerializeField] private List<GameObject> items = new List<GameObject>();
	[SerializeField] private int itemsNum;
	[SerializeField] private GameObject itemObjectPrefub;

	private List<Area> areas = new List<Area>();
	private List<Vector2> emptyPoses = new List<Vector2>();

	private Vector2Int AreaSize;
	private Grid grid;

	private PlayingFieldBehaviour fieldBehaviour;

	static public int[][] board;
	static public Dictionary<(int, int), GameObject> Icons = new Dictionary<(int, int), GameObject>();

	static public UnityEvent OnBoardFilled = new UnityEvent();

	// Start is called before the first frame update
	void Start()
	{
		string Level = PlayerPrefs.GetString("PlayerLevel");

		switch (Level)
		{
			case "EASY": itemsNum = 4; break;
			case "MIDDLE": itemsNum = 6; break;
			case "HARD": itemsNum = 7; break;
		}

		Icons.Clear();

		fieldBehaviour = GetComponent<PlayingFieldBehaviour>();
		grid = GetComponent<Grid>();
		AreaSize = new Vector2Int((int)fieldBehaviour.AreaSize.x, (int)fieldBehaviour.AreaSize.y);

		GenerateIconArea();

		foreach(var area in areas)
		{
			(Cell, Cell) pairCells = FindFarestCells(area);
			SpawnItems(pairCells.Item1, pairCells.Item2);
		}

		ClearEmptyCellBoard();

		OnBoardFilled.Invoke();

		//for (int i = 0; i < board.GetLength(0); i++)
		//{
		//	for (int j = 0; j < board[i].GetLength(0); j++)
		//	{
		//		var item = Instantiate(items[board[i][j]], transform);
		//		item.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(i, j, 0));
		//	}
		//}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void ClearEmptyCellBoard()
	{
		foreach(var pos in emptyPoses)
		{
			board[(int)pos.x][(int)pos.y] = 0;
		}
	}

	private void SpawnItems(Cell cell1, Cell cell2)
	{
		var icon1 = SpawnItem(cell1);
		var icon2 = SpawnItem(cell2);

		var itemObject = Instantiate(itemObjectPrefub, transform);
		itemObject.name = items[board[cell1.x][cell1.y]].name + " object";
		var LineRenderer = itemObject.GetComponent<LineRenderer>();
		Color color = icon1.GetComponent<IconBehaviour>().iconColor;
		color.a = 1;
		LineRenderer.startColor = color;
		LineRenderer.endColor = color;

		icon1.transform.SetParent(itemObject.transform);
		icon2.transform.SetParent(itemObject.transform);

		Icons[(cell1.x, cell1.y)] = itemObject;
		Icons[(cell2.x, cell2.y)] = itemObject;
	}

	private GameObject SpawnItem(Cell cell)
	{
		var item = Instantiate(items[board[cell.x][cell.y]], transform);
		item.transform.localPosition = grid.GetCellCenterLocal(new Vector3Int(cell.x, cell.y, 0));
		emptyPoses.Remove(new Vector2(cell.x, cell.y));

		return item;
	}

	private (Cell, Cell) FindFarestCells(Area area)
	{
		Cell cell1 = new Cell(Vector2.zero, 0), cell2 = new Cell(Vector2.zero, 0);
		float maxDist = 0;

		for (int i = 0; i < area.cells.Count - 1; i++)
		{
			for (int j = 1; j < area.cells.Count; j++)
			{
				Cell tempCell1, tempCell2;
				float dist = 0;

				tempCell1 = area.cells[i];
				tempCell2 = area.cells[j];

				dist = Vector2.Distance(new Vector2(tempCell1.x, tempCell1.y), new Vector2(tempCell2.x, tempCell2.y));

				if (dist > maxDist)
				{
					cell1 = tempCell1;
					cell2 = tempCell2;
					maxDist = dist;
				}
			}
		}

		return (cell1, cell2);
	}

	private void GenerateIconArea()
	{
		bool isCorrectSpawn;
		do
		{
			isCorrectSpawn = true;
			emptyPoses.Clear();
			areas.Clear();
			InitializeBoard();

			for (int i = 0; i < board.GetLength(0); i++)
			{
				for (int j = 0; j < board[i].GetLength(0); j++)
				{
					emptyPoses.Add(new Vector2(i, j));
				}
			}

			for (int i = 0; i < itemsNum; ++i)
			{
				Vector2 pos = emptyPoses[Random.Range(0, emptyPoses.Count())];
				emptyPoses.Remove(pos);
				Area area = new Area(pos, i + 1);
				areas.Add(area);
			}

			while (board.Any(x => x.Any(y => y == 0)) && isCorrectSpawn)
			{
				foreach (var area in areas)
				{
					if (!area.TrySpredArea())
					{
						isCorrectSpawn = false;
						break;
					}
				}
			}

		} while (!isCorrectSpawn);
		emptyPoses.Clear();
		for (int i = 0; i < board.GetLength(0); i++)
		{
			for (int j = 0; j < board[i].GetLength(0); j++)
			{
				emptyPoses.Add(new Vector2(i, j));
			}
		}
	}

	private void InitializeBoard()
	{
		board = new int[AreaSize.x][];
		for (int i = 0; i < board.GetLength(0); i++)
		{
			board[i] = new int[AreaSize.y];
			for(int j = 0; j < board[i].GetLength(0); j++)
			{
				board[i][j] = 0;
			}
		}
	}
}

class Cell
{
	public int AreaNum;

	public int x;
	public int y;

	public bool[] Steps = new bool[4];

	public Cell(Vector2 pos, int areaNum)
	{
		this.x = (int)pos.x;
		this.y = (int)pos.y;
		this.AreaNum = areaNum;
	}

	public bool isCanStep()
	{
		return Steps.Any(x => x == true);
	}

	public void SetSteps()
	{
		if (x - 1 >= 0)
		{
			Steps[0] = BoardBehaviour.board[x - 1][y] == 0;
		}

		if (y - 1 >= 0)
		{
			Steps[1] = BoardBehaviour.board[x][y - 1] == 0;
		}

		if (x + 1 < BoardBehaviour.board.GetLength(0))
		{
			Steps[2] = BoardBehaviour.board[x + 1][y] == 0;
		}

		if (y + 1 < BoardBehaviour.board[0].GetLength(0))
		{
			Steps[3] = BoardBehaviour.board[x][y + 1] == 0;
		}
	}
}

class Area
{
	public List<Cell> cells = new List<Cell>();
	
	private List<Cell> cellsCanMove = new List<Cell>();

	public Area(Vector2 pos, int areaNum)
	{
		Cell cell = new Cell(pos, areaNum);
		cells.Add(cell);

		BoardBehaviour.board[cell.x][cell.y] = areaNum;
	}

	public bool TrySpredArea()
	{
		cellsCanMove.Clear();

		for (int i = 0; i < cells.Count; i++)
		{
			cells[i].SetSteps();
			if(cells[i].isCanStep())
			{
				cellsCanMove.Add(cells[i]);
			}
		}

		if(cellsCanMove.Count > 0) { 
			Cell stepCell = cellsCanMove[Random.Range(0, cellsCanMove.Count)];

			var possibleSteps = stepCell.Steps.Where(x => x == true).ToList();
			int stepIndex = Array.IndexOf(stepCell.Steps, possibleSteps[Random.Range(0, possibleSteps.Count())]);

			Vector2 pos = Vector2.zero;

			if (stepIndex == 0)
			{
				pos = Vector2.left;
			}

			if (stepIndex == 1)
			{
				pos = Vector2.down;
			}

			if (stepIndex == 2)
			{
				pos = Vector2.right;
			}

			if (stepIndex == 3)
			{
				pos = Vector2.up;
			}
			
			Cell newCell = new Cell(new Vector2(stepCell.x, stepCell.y) + pos, stepCell.AreaNum);
			cells.Add(newCell);

			BoardBehaviour.board[newCell.x][newCell.y] = newCell.AreaNum;
		}
		else if(cells.Count < 3)
		{
			return false;
		}

		return true;
	}
}
