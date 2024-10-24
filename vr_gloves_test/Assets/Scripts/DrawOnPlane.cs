using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnPlane : MonoBehaviour
{
    public Camera cam; // ������ ��� ����������� ����������� �����
    public GameObject penTip; // ������ �����, ������� ����� ��������
    public Texture2D drawingTexture; // �������� ��� ���������
    public Color penColor = Color.black; // ���� �����
    public int penSize = 5; // ������ �����

    private List<Vector2> points = new List<Vector2>(); // ������ ����� ����������
    private bool isDrawing = false; // ���� ���������
    private Renderer planeRenderer;

    void Start()
    {
        planeRenderer = GetComponent<Renderer>();
        drawingTexture = new Texture2D(1024, 1024); // ������� ��������
        planeRenderer.material.mainTexture = drawingTexture; // ����������� �������� � ���������
        ClearTexture();
    }

    void Update()
    {
        // ������� ��� �� ����� � ���������
        Ray ray = new Ray(penTip.transform.position, -penTip.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ���������, ������ �� ����� �� ���������
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uv = new Vector2(hit.textureCoord.x * drawingTexture.width, hit.textureCoord.y * drawingTexture.height);

                // ���� ����� �������� ���������, �� �������� ���������
                if (!isDrawing)
                {
                    points.Clear(); // ������� ���������� �����
                    isDrawing = true;
                }

                // ��������� ����� � ������ � ������ �����
                points.Add(uv);
                if (points.Count > 1)
                {
                    DrawLine(points[points.Count - 2], uv);
                }
            }
        }
        else
        {
            // ���� ����� ������ �� �������� ���������, ������������� ���������
            if (isDrawing)
            {
                isDrawing = false;
                StartCoroutine(DetectShape()); // ��������� �������� ��� ������� �����
            }
        }
    }

    IEnumerator DetectShape()
    {
        // ��������� ����� ����� ���������� ���������
        yield return new WaitForSeconds(0.1f);

        if (points.Count < 10)
        {
            // ���� ������� ���� �����, �� �����������
            yield break;
        }

        // ���������, ������ �� �� ����
        if (IsCircle(points))
        {
            Debug.Log("��������� ����!");
            DrawPerfectCircle(points);
        }
        // ���������, ������ �� �� �������
        else if (IsSquare(points))
        {
            Debug.Log("��������� �������!");
            DrawPerfectSquare(points);
        }

        drawingTexture.Apply(); // ��������� ��������� � ��������
    }

    // ������� ��� ��������, �������� �� ������������ ������
    bool IsCircle(List<Vector2> points)
    {
        Vector2 center = GetCenter(points);
        float averageRadius = 0;

        // ��������� ������� ������
        foreach (var point in points)
        {
            averageRadius += Vector2.Distance(center, point);
        }
        averageRadius /= points.Count;

        float variance = 0;
        foreach (var point in points)
        {
            float distance = Vector2.Distance(center, point);
            variance += Mathf.Abs(distance - averageRadius);
        }
        variance /= points.Count;

        // ���� ������� ���������� �� ������ �� ���� ����� �������� ���������
        return variance < 10f; // ����� ��� ����������� �����
    }

    // ������� ��� ��������, �������� �� ������������ ���������
    bool IsSquare(List<Vector2> points)
    {
        // ���������� ����������� � ������������ X � Y, ����� ���������� ������������
        Vector2 min = points[0];
        Vector2 max = points[0];

        foreach (var point in points)
        {
            if (point.x < min.x) min.x = point.x;
            if (point.y < min.y) min.y = point.y;
            if (point.x > max.x) max.x = point.x;
            if (point.y > max.y) max.y = point.y;
        }

        float width = max.x - min.x;
        float height = max.y - min.y;

        // ���������, ��� ������� ����� ���������
        return Mathf.Abs(width - height) < 10f; // ����� ��� ����������� ��������
    }

    // ��������� ������ ����������
    Vector2 GetCenter(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (var point in points)
        {
            sum += point;
        }
        return sum / points.Count;
    }

    // ������������� � ��������� ���������� �����
    void DrawPerfectCircle(List<Vector2> points)
    {
        Vector2 center = GetCenter(points);
        float radius = 0f;

        foreach (var point in points)
        {
            radius += Vector2.Distance(center, point);
        }
        radius /= points.Count;

        int r = Mathf.RoundToInt(radius);

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int pixelX = Mathf.Clamp((int)center.x + x, 0, drawingTexture.width - 1);
                    int pixelY = Mathf.Clamp((int)center.y + y, 0, drawingTexture.height - 1);
                    drawingTexture.SetPixel(pixelX, pixelY, penColor);
                }
            }
        }
    }

    // ������������� � ��������� ���������� ��������
    void DrawPerfectSquare(List<Vector2> points)
    {
        Vector2 min = points[0];
        Vector2 max = points[0];

        foreach (var point in points)
        {
            if (point.x < min.x) min.x = point.x;
            if (point.y < min.y) min.y = point.y;
            if (point.x > max.x) max.x = point.x;
            if (point.y > max.y) max.y = point.y;
        }

        float size = Mathf.Min(max.x - min.x, max.y - min.y);

        Vector2 bottomLeft = new Vector2(min.x, min.y);
        Vector2 topRight = new Vector2(min.x + size, min.y + size);

        // ������ ������� ��������
        DrawLine(bottomLeft, new Vector2(bottomLeft.x, topRight.y));
        DrawLine(bottomLeft, new Vector2(topRight.x, bottomLeft.y));
        DrawLine(new Vector2(bottomLeft.x, topRight.y), topRight);
        DrawLine(new Vector2(topRight.x, bottomLeft.y), topRight);
    }

    // ����� ��� ��������� �����
    void DrawLine(Vector2 from, Vector2 to)
    {
        int x0 = Mathf.RoundToInt(from.x);
        int y0 = Mathf.RoundToInt(from.y);
        int x1 = Mathf.RoundToInt(to.x);
        int y1 = Mathf.RoundToInt(to.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawCirclePixel(x0, y0, penSize);

            if (x0 == x1 && y0 == y1) break;
            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        drawingTexture.Apply();
    }

    // ����� ��� ��������� "�������" � ���� ����� (��� �����)
    void DrawCirclePixel(int x, int y, int radius)
    {
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i * i + j * j <= radius * radius)
                {
                    int pixelX = Mathf.Clamp(x + i, 0, drawingTexture.width - 1);
                    int pixelY = Mathf.Clamp(y + j, 0, drawingTexture.height - 1);
                    drawingTexture.SetPixel(pixelX, pixelY, penColor);
                }
            }
        }
    }

    // ����� ��� ������� ��������
    public void ClearTexture()
    {
        for (int x = 0; x < drawingTexture.width; x++)
        {
            for (int y = 0; y < drawingTexture.height; y++)
            {
                drawingTexture.SetPixel(x, y, Color.white); // ��������� ��� �������� ����� ������
            }
        }
        drawingTexture.Apply(); // ��������� ��������� � ��������
    }
}