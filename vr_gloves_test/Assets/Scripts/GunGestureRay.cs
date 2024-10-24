/*using System.Collections;
using SG;
using SG.XR;
using UnityEngine;

public class GunGestureRay : MonoBehaviour
{
    public SG_TrackedHand senseGloveController; // ������ �� ���������� ��������
    private bool isPistolGesture = false; // ���� ��� ������������ �������� ��������� �����
    private LineRenderer laserBeam; // ��������� ��� ����������� ����
    private bool isThumbBent = false; // ���� ��� ������������ ��������� �������� ������

    void Start()
    {
        // ������� ����� LineRenderer ��� ����������� ����
        laserBeam = gameObject.AddComponent<LineRenderer>();
        laserBeam.material = new Material(Shader.Find("Sprites/Default"));
        laserBeam.startWidth = 0.01f;
        laserBeam.endWidth = 0.01f;
        laserBeam.startColor = Color.red; // ������� ���� ���� �� ���������
        laserBeam.endColor = Color.red;
        laserBeam.positionCount = 2;
        laserBeam.enabled = false; // ��� �� ����� �� ���������
    }

    void Update()
    {
        // ��������� ���������� �� ��������
        if (senseGloveController != null && senseGloveController.IsConnected)
        {
            // �������� ��������� �������� �������
            var fingerStates = senseGloveController.GetFingerStates();

            // ��������� ������������ ����� "��������"
            bool thumbUp = fingerStates[0].jointStates[1].angle > 70f; // ������� ����� �����
            bool indexStraight = fingerStates[1].jointStates[1].angle < 30f; // ������������ ����� ������
            bool middleStraight = fingerStates[2].jointStates[1].angle < 30f; // ������� ����� ������
            bool ringBent = fingerStates[3].jointStates[1].angle > 70f; // ���������� ����� ������
            bool pinkyBent = fingerStates[4].jointStates[1].angle > 70f; // ������� ������

            // ���� ���� ������������� "���������"
            if (thumbUp && indexStraight && middleStraight && ringBent && pinkyBent)
            {
                if (!isPistolGesture)
                {
                    isPistolGesture = true; // ������������� ���� � true
                }

                // ���������� ���
                laserBeam.enabled = true;

                // ������������� ������ ���� �� ������� ������������� ������
                Vector3 fingerTipPosition = senseGloveController.GetFingerTipPosition(1);
                laserBeam.SetPosition(0, fingerTipPosition);

                // ������������� ����� ���� �� ���������� 10 ������ �� ������
                Vector3 beamEndPosition = fingerTipPosition + senseGloveController.GetHandForward() * 10f;
                laserBeam.SetPosition(1, beamEndPosition);

                // ��������� ��������� �������� ������
                isThumbBent = fingerStates[0].jointStates[1].angle < 30f; // ������� ����� ������

                // ���� ������� ����� ������, ������ ���� ���� �� �����
                if (isThumbBent)
                {
                    laserBeam.startColor = Color.blue;
                    laserBeam.endColor = Color.blue;
                }
                else // ����� ������ ���� ���� �� �������
                {
                    laserBeam.startColor = Color.red;
                    laserBeam.endColor = Color.red;
                }
            }
            else
            {
                // ���� ���� �� ������������� "���������", �������� ���
                isPistolGesture = false;
                laserBeam.enabled = false;
            }
        }
    }
}*/