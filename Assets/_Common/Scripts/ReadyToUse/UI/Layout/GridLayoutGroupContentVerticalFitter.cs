using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutGroupContentVerticalFitter : MonoBehaviour
{
	private GridLayoutGroup gridLayoutGroup = null;
	private RectTransform rectTransform = null;

	private void Init()
	{
		gridLayoutGroup = GetComponent<GridLayoutGroup>();
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (gridLayoutGroup == null)
			Init();

		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.childCount * gridLayoutGroup.cellSize.y + (rectTransform.childCount + 1) * gridLayoutGroup.spacing.y + gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom);
	}
}
