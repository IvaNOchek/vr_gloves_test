/*using System.Collections;
using SG;
using SG.XR;
using UnityEngine;

public class GunGestureRay : MonoBehaviour
{
    public SG_TrackedHand senseGloveController; // Ссылки на контроллер перчатки
    private bool isPistolGesture = false; // Флаг для отслеживания текущего состояния жеста
    private LineRenderer laserBeam; // Компонент для отображения луча
    private bool isThumbBent = false; // Флаг для отслеживания состояния большого пальца

    void Start()
    {
        // Создаем новый LineRenderer для отображения луча
        laserBeam = gameObject.AddComponent<LineRenderer>();
        laserBeam.material = new Material(Shader.Find("Sprites/Default"));
        laserBeam.startWidth = 0.01f;
        laserBeam.endWidth = 0.01f;
        laserBeam.startColor = Color.red; // Красный цвет луча по умолчанию
        laserBeam.endColor = Color.red;
        laserBeam.positionCount = 2;
        laserBeam.enabled = false; // Луч не виден по умолчанию
    }

    void Update()
    {
        // Проверяем подключена ли перчатка
        if (senseGloveController != null && senseGloveController.IsConnected)
        {
            // Получаем состояния суставов пальцев
            var fingerStates = senseGloveController.GetFingerStates();

            // Проверяем соответствие жесту "пистолет"
            bool thumbUp = fingerStates[0].jointStates[1].angle > 70f; // Большой палец вверх
            bool indexStraight = fingerStates[1].jointStates[1].angle < 30f; // Указательный палец прямой
            bool middleStraight = fingerStates[2].jointStates[1].angle < 30f; // Средний палец прямой
            bool ringBent = fingerStates[3].jointStates[1].angle > 70f; // Безымянный палец согнут
            bool pinkyBent = fingerStates[4].jointStates[1].angle > 70f; // Мизинец согнут

            // Если жест соответствует "пистолету"
            if (thumbUp && indexStraight && middleStraight && ringBent && pinkyBent)
            {
                if (!isPistolGesture)
                {
                    isPistolGesture = true; // Устанавливаем флаг в true
                }

                // Показываем луч
                laserBeam.enabled = true;

                // Устанавливаем начало луча на кончике указательного пальца
                Vector3 fingerTipPosition = senseGloveController.GetFingerTipPosition(1);
                laserBeam.SetPosition(0, fingerTipPosition);

                // Устанавливаем конец луча на расстоянии 10 единиц от начала
                Vector3 beamEndPosition = fingerTipPosition + senseGloveController.GetHandForward() * 10f;
                laserBeam.SetPosition(1, beamEndPosition);

                // Проверяем состояние большого пальца
                isThumbBent = fingerStates[0].jointStates[1].angle < 30f; // Большой палец согнут

                // Если большой палец согнут, меняем цвет луча на синий
                if (isThumbBent)
                {
                    laserBeam.startColor = Color.blue;
                    laserBeam.endColor = Color.blue;
                }
                else // Иначе меняем цвет луча на красный
                {
                    laserBeam.startColor = Color.red;
                    laserBeam.endColor = Color.red;
                }
            }
            else
            {
                // Если жест не соответствует "пистолету", скрываем луч
                isPistolGesture = false;
                laserBeam.enabled = false;
            }
        }
    }
}*/