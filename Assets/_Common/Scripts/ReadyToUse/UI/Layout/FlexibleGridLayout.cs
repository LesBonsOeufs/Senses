using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : GridLayoutGroup
{
	[SerializeField] private int _childCount;
	[SerializeField] private int _columnCount;
	[SerializeField] private int _rowCount;

	public override void SetLayoutHorizontal()
	{
		UpdateCellSize();
		base.SetLayoutHorizontal();
	}
	public override void SetLayoutVertical()
	{
		UpdateCellSize();
		base.SetLayoutVertical();
	}

	private void UpdateCellSize()
	{
		_childCount = transform.childCount;
		switch (constraint)
		{
			case Constraint.FixedColumnCount:
				_columnCount = constraintCount;
				_rowCount = Mathf.Max(1, _childCount / _columnCount);
				break;

			case Constraint.FixedRowCount:
				_rowCount = constraintCount;
				_columnCount = Mathf.Max(1, _childCount / _rowCount);
				break;

			default: return;
		}

		float x = (rectTransform.rect.size.x - padding.horizontal - spacing.x * (_columnCount - 1)) / _columnCount;
		//float y = (rectTransform.rect.size.y - padding.vertical - spacing.y * (_rowCount - 1)) / _rowCount;
		this.cellSize = new Vector2(x, this.cellSize.y);
	}
}
