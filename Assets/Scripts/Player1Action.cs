using UnityEngine;
using Photon.Pun;
using System.Collections;

public class SpriteChanger : MonoBehaviour
{
    private PhotonView view;
    public float jumpDistance = 2f; // Публичная переменная для расстояния отскока
    private Vector2 originalPosition;

    // Добавляем переменную для кулдауна
    public float moveCooldown = 1f; // Время кулдауна в секундах
    private float lastMoveTime; // Время последнего перемещения

    void Start() 
    {
        view = GetComponent<PhotonView>();
        originalPosition = transform.position; // Сохраняем исходную позицию
        lastMoveTime = 0f; // Инициализируем время последнего перемещения
    }

    void Update()
    {
        // Проверяем, является ли этот объект локальным
        if (view.IsMine) 
        {
            // Проверяем нажатие клавиш и кулдаун
            if (Input.GetKeyDown(KeyCode.Z) && Time.time >= lastMoveTime + moveCooldown)
            {
                // Первый игрок движется влево
                Move(Vector2.left);
            }
            else if (Input.GetKeyDown(KeyCode.X) && Time.time >= lastMoveTime + moveCooldown)
            {
                // Первый игрок движется вправо
                Move(Vector2.right);
            }
        }
    }

    private void Move(Vector2 direction)
    {
        // Запускаем корутину для перемещения и возврата
        StartCoroutine(MoveAndReturn(direction));
    }

    private IEnumerator MoveAndReturn(Vector2 direction)
    {
        // Вычисляем новую позицию
        Vector2 targetPosition = (Vector2)transform.position + direction * jumpDistance;
        // Двигаем персонажа
        transform.position = targetPosition;

        // Обновляем время последнего перемещения
        lastMoveTime = Time.time;

        // Ждем 1 секунду
        yield return new WaitForSeconds(1f);
        // Возвращаем персонажа в исходную позицию
        transform.position = originalPosition;
    }
}