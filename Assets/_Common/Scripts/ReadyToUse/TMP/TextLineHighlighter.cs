using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLineHighlighter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isOverlay = true;
    [SerializeField] private Color _highlightColor = Color.yellow;

    private TextMeshProUGUI _tmpText;
    private Camera _mainCamera;
    private Color _defaultColor;
    private HashSet<int> _currentHighlight = new HashSet<int>();

    private void Awake()
    {
        _tmpText = GetComponent<TextMeshProUGUI>();
        _mainCamera = isOverlay ? null : Camera.main;
        _defaultColor = _tmpText.color;
    }

    public void UpdateHighlight(Vector2 startScreenPos, Vector2 endScreenPos)
    {
        _tmpText.ForceMeshUpdate();

        int startIndex = TMP_TextUtilities.FindNearestCharacter(_tmpText, startScreenPos, _mainCamera, false);
        int endIndex = TMP_TextUtilities.FindNearestCharacter(_tmpText, endScreenPos, _mainCamera, false);

        Debug.Log("Start: " + startIndex);
        Debug.Log("End: " + endIndex);

        if (startIndex == -1 || endIndex == -1) return;

        if (TryGetGridPosition(startIndex, out Vector2Int startGrid) &&
            TryGetGridPosition(endIndex, out Vector2Int endGrid))
        {
            var linePoints = GetBresenhamLine(startGrid, endGrid);
            UpdateHighlightedCharacters(linePoints);
        }
    }

    public void ClearHighlight()
    {
        ResetColors();
        _currentHighlight.Clear();
    }

    private bool TryGetGridPosition(int charIndex, out Vector2Int gridPosition)
    {
        gridPosition = Vector2Int.zero;

        if (charIndex < 0 || charIndex >= _tmpText.textInfo.characterCount)
            return false;

        for (int i = 0; i < _tmpText.textInfo.lineCount; i++)
        {
            TMP_LineInfo line = _tmpText.textInfo.lineInfo[i];
            if (charIndex >= line.firstCharacterIndex && charIndex <= line.lastCharacterIndex)
            {
                gridPosition.x = charIndex - line.firstCharacterIndex; // Column
                gridPosition.y = i; // Row
                return true;
            }
        }
        return false;
    }

    private List<Vector2Int> GetBresenhamLine(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = start.x < end.x ? 1 : -1;
        int sy = start.y < end.y ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            points.Add(new Vector2Int(start.x, start.y));

            if (start.x == end.x && start.y == end.y) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                start.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                start.y += sy;
            }
        }
        return points;
    }

    private void UpdateHighlightedCharacters(List<Vector2Int> gridPoints)
    {
        HashSet<int> newIndices = new HashSet<int>();

        foreach (var point in gridPoints)
        {
            if (point.y >= _tmpText.textInfo.lineCount) continue;

            TMP_LineInfo line = _tmpText.textInfo.lineInfo[point.y];
            int charIndex = line.firstCharacterIndex + point.x;

            if (charIndex <= line.lastCharacterIndex && charIndex < _tmpText.textInfo.characterCount)
            {
                newIndices.Add(charIndex);
            }
        }

        UpdateColors(newIndices);
        _currentHighlight = newIndices;
    }

    private void UpdateColors(HashSet<int> indices)
    {
        _tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = _tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            Color32 color = indices.Contains(i) ? _highlightColor : _defaultColor;

            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;
        }

        _tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ResetColors()
    {
        UpdateColors(new HashSet<int>());
    }
}
