using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnPlane : MonoBehaviour
{
    public Camera cam; // Камера для определения направления лучей
    public GameObject penTip; // Объект ручки, который будет рисовать
    public Texture2D drawingTexture; // Текстура для рисования
    public Color penColor = Color.black; // Цвет ручки
    public int penSize = 5; // Размер кисти

    private List<Vector2> points = new List<Vector2>(); // Список точек траектории
    private bool isDrawing = false; // Флаг рисования
    private Renderer planeRenderer;

    void Start()
    {
        planeRenderer = GetComponent<Renderer>();
        drawingTexture = new Texture2D(1024, 1024); // Создаем текстуру
        planeRenderer.material.mainTexture = drawingTexture; // Привязываем текстуру к материалу
        ClearTexture();
    }

    void Update()
    {
        // Кастуем луч от ручки к плоскости
        Ray ray = new Ray(penTip.transform.position, -penTip.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Проверяем, попала ли ручка на плоскость
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uv = new Vector2(hit.textureCoord.x * drawingTexture.width, hit.textureCoord.y * drawingTexture.height);

                // Если ручка касается плоскости, то начинаем рисование
                if (!isDrawing)
                {
                    points.Clear(); // Очищаем предыдущие точки
                    isDrawing = true;
                }

                // Добавляем точки в список и рисуем линию
                points.Add(uv);
                if (points.Count > 1)
                {
                    DrawLine(points[points.Count - 2], uv);
                }
            }
        }
        else
        {
            // Если ручка больше не касается плоскости, останавливаем рисование
            if (isDrawing)
            {
                isDrawing = false;
                StartCoroutine(DetectShape()); // Запускаем корутину для анализа формы
            }
        }
    }

    IEnumerator DetectShape()
    {
        // Проверяем форму после завершения рисования
        yield return new WaitForSeconds(0.1f);

        if (points.Count < 10)
        {
            // Если слишком мало точек, не анализируем
            yield break;
        }

        // Проверяем, похоже ли на круг
        if (IsCircle(points))
        {
            Debug.Log("Обнаружен круг!");
            DrawPerfectCircle(points);
        }
        // Проверяем, похоже ли на квадрат
        else if (IsSquare(points))
        {
            Debug.Log("Обнаружен квадрат!");
            DrawPerfectSquare(points);
        }

        drawingTexture.Apply(); // Применяем изменения к текстуре
    }

    // Функция для проверки, является ли нарисованное кругом
    bool IsCircle(List<Vector2> points)
    {
        Vector2 center = GetCenter(points);
        float averageRadius = 0;

        // Вычисляем средний радиус
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

        // Если среднее расстояние от центра до всех точек примерно одинаково
        return variance < 10f; // Порог для определения круга
    }

    // Функция для проверки, является ли нарисованное квадратом
    bool IsSquare(List<Vector2> points)
    {
        // Используем минимальный и максимальный X и Y, чтобы определить квадратность
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

        // Проверяем, что стороны почти одинаковы
        return Mathf.Abs(width - height) < 10f; // Порог для определения квадрата
    }

    // Получение центра траектории
    Vector2 GetCenter(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (var point in points)
        {
            sum += point;
        }
        return sum / points.Count;
    }

    // Корректировка и отрисовка идеального круга
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

    // Корректировка и отрисовка идеального квадрата
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

        // Рисуем стороны квадрата
        DrawLine(bottomLeft, new Vector2(bottomLeft.x, topRight.y));
        DrawLine(bottomLeft, new Vector2(topRight.x, bottomLeft.y));
        DrawLine(new Vector2(bottomLeft.x, topRight.y), topRight);
        DrawLine(new Vector2(topRight.x, bottomLeft.y), topRight);
    }

    // Метод для рисования линии
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

    // Метод для рисования "пикселя" в виде круга (как кисть)
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

    // Метод для очистки текстуры
    public void ClearTexture()
    {
        for (int x = 0; x < drawingTexture.width; x++)
        {
            for (int y = 0; y < drawingTexture.height; y++)
            {
                drawingTexture.SetPixel(x, y, Color.white); // Заполняем всю текстуру белым цветом
            }
        }
        drawingTexture.Apply(); // Применяем изменения к текстуре
    }
}