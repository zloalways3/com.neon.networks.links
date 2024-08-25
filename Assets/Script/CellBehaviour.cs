using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CellBehaviour : MonoBehaviour
{
	public bool isCellFilled;
	public int iconNum;

	private PlayingFieldBehaviour fieldBehaviour;
	private IconBehaviour icon;
	private GameObject iconObject;

	private bool isBoardFilled;

	public static UnityEvent OnLineDrawing = new UnityEvent();

	// Start is called before the first frame update
	void Awake()
    {
		isBoardFilled = false;
        fieldBehaviour = transform.parent.GetComponent<PlayingFieldBehaviour>();

		BoardBehaviour.OnBoardFilled.AddListener(OnBoardFilled);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnBoardFilled()
	{
		isBoardFilled = true;
		Debug.Log("Board Filled");
	}

	private void OnMouseDown()
	{
		if (!isBoardFilled)
			return;

		Vector3Int pos = fieldBehaviour.grid.WorldToCell(transform.position);

		if (BoardBehaviour.board[pos.x][pos.y] != 0)
		{
			iconNum = BoardBehaviour.board[pos.x][pos.y];
			//isCellFilled = true;
			iconObject = BoardBehaviour.Icons[(pos.x, pos.y)];
			icon = iconObject.transform.GetChild(0).GetComponent<IconBehaviour>();
			icon.lineRenderer = iconObject.GetComponent<LineRenderer>();

			if (icon.lineRenderer.positionCount > 0)
			{
				Vector3[] positions = new Vector3[icon.lineRenderer.positionCount];
				icon.lineRenderer.GetPositions(positions);
				foreach (var position in positions)
				{
					Vector3Int posToClean = fieldBehaviour.grid.WorldToCell(position);
					if (!BoardBehaviour.Icons.ContainsKey((posToClean.x, posToClean.y)))
					{
						BoardBehaviour.board[posToClean.x][posToClean.y] = 0;
					}
				}
				icon.lineRenderer.positionCount = 0;
				iconObject.GetComponent<IconObjectBehaviour>().SetLineComplete(false);
			}

			icon.lineRenderer.positionCount = 1;
			icon.lineRenderer.SetPosition(0, transform.position);
			OnLineDrawing.Invoke();
		}
	}

	private void OnMouseDrag()
	{
		if (!isBoardFilled)
			return;

		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (hit.collider != null)
		{
			if (!hit.collider.gameObject.GetComponent<CellBehaviour>().isCellFilled)
			{
				Vector3[] positions = new Vector3[icon.lineRenderer.positionCount];
				icon.lineRenderer.GetPositions(positions);
				Vector3Int pos = fieldBehaviour.grid.WorldToCell(hit.collider.transform.position);
				if (Vector3.Distance(fieldBehaviour.grid.WorldToCell(positions[positions.Length - 1]), pos) == 1)
				{
					//Debug.Log("BoardBehaviour.board[pos.x][pos.y]: " + BoardBehaviour.board[pos.x][pos.y]);
					//Debug.Log("BoardBehaviour.Icons.ContainsKey((pos.x, pos.y)): " + BoardBehaviour.Icons.ContainsKey((pos.x, pos.y)));
					//if (BoardBehaviour.Icons.ContainsKey((pos.x, pos.y)))
					//{
					//	Debug.Log("BoardBehaviour.Icons[(pos.x, pos.y)].name: " + BoardBehaviour.Icons[(pos.x, pos.y)].name);
					//	Debug.Log("BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0).GetComponent<IconBehaviour>().iconColor.Equals(icon.iconColor): " + BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0).GetComponent<IconBehaviour>().iconColor.Equals(icon.iconColor));
					//	Debug.Log("BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0): " + BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0));
					//	Debug.Log("BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0).GetComponent<IconBehaviour>(): " + BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0).GetComponent<IconBehaviour>());
					//}
					//Debug.Log("hit.transform.position != positions[0]: " + (hit.transform.position != positions[0]));
					//Debug.Log("positions.Where(x => x == hit.collider.transform.position).Count(): " + positions.Where(x => x == hit.collider.transform.position).Count());
					if ((BoardBehaviour.board[pos.x][pos.y] == 0 || 
						(BoardBehaviour.Icons.ContainsKey((pos.x, pos.y)) &&
						BoardBehaviour.Icons[(pos.x, pos.y)].transform.GetChild(0).GetComponent<IconBehaviour>().iconColor.Equals(icon.iconColor))) &&
						hit.transform.position != positions[0] &&
						positions.Where(x => x == hit.collider.transform.position).Count() == 0)
					{
						icon.lineRenderer.positionCount = icon.lineRenderer.positionCount + 1;
						icon.lineRenderer.SetPosition(icon.lineRenderer.positionCount - 1, hit.collider.transform.position);
						BoardBehaviour.board[pos.x][pos.y] = iconNum;
						OnLineDrawing.Invoke();
					}
					else if(BoardBehaviour.board[pos.x][pos.y] == iconNum)
					{
						var prevPos = fieldBehaviour.grid.WorldToCell(positions[positions.Length - 1]);
						if (!BoardBehaviour.Icons.ContainsKey((prevPos.x, prevPos.y)))
						{
							BoardBehaviour.board[prevPos.x][prevPos.y] = 0;
						}
						icon.lineRenderer.positionCount = icon.lineRenderer.positionCount - 1;
					}
					else if (hit.transform.position == positions[0])
					{
						foreach (var tempPos in positions)
						{
							Vector3Int posToClean = fieldBehaviour.grid.WorldToCell(tempPos);
							if (!BoardBehaviour.Icons.ContainsKey((posToClean.x, posToClean.y)))
							{
								BoardBehaviour.board[posToClean.x][posToClean.y] = 0;
							}
						}
						icon.lineRenderer.positionCount = 0;
					}
				}
			}
		}
	}

	private void OnMouseUp()
	{
		if (!isBoardFilled)
			return;

		Vector3[] positions = new Vector3[icon.lineRenderer.positionCount];
		icon.lineRenderer.GetPositions(positions);
		Vector3Int pos1 = fieldBehaviour.grid.WorldToCell(positions[0]);
		Vector3Int pos2 = fieldBehaviour.grid.WorldToCell(positions[positions.Length - 1]);
		if (BoardBehaviour.Icons.ContainsKey((pos1.x, pos1.y)) &&
			BoardBehaviour.Icons.ContainsKey((pos2.x, pos2.y)) &&
			positions[0] != positions[positions.Length - 1])
		{
			if (BoardBehaviour.Icons[(pos1.x, pos1.y)].transform.GetChild(0).GetComponent<IconBehaviour>().iconColor.Equals(icon.iconColor) &&
			BoardBehaviour.Icons[(pos2.x, pos2.y)].transform.GetChild(0).GetComponent<IconBehaviour>().iconColor.Equals(icon.iconColor))
			{
				iconObject.GetComponent<IconObjectBehaviour>().SetLineComplete(true);
			}
		}
		else
		{
			foreach(var pos in positions)
			{
				Vector3Int posToClean = fieldBehaviour.grid.WorldToCell(pos);
				if (!BoardBehaviour.Icons.ContainsKey((posToClean.x, posToClean.y)))
				{ 
					BoardBehaviour.board[posToClean.x][posToClean.y] = 0;
				}
			}
			icon.lineRenderer.positionCount = 0;
		}
	}
}
