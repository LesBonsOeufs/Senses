using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(GridLayoutGroup))]
public class FullWidthGridLayout : MonoBehaviour
{
	private GridLayoutGroup gridLayoutGroup = null;
	private RectTransform rectTransform = null;

	private void Init()
	{
		gridLayoutGroup = GetComponent<GridLayoutGroup>();
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (gridLayoutGroup == null)
			Init();

		gridLayoutGroup.cellSize = new Vector2(rectTransform.rect.width - (gridLayoutGroup.padding.left + gridLayoutGroup.padding.right), gridLayoutGroup.cellSize.y);
	}
}