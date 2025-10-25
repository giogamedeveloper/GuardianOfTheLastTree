using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPoisonDrop : MonoBehaviour
{
    public GameObject poisonPrefab;

    public float dropInterval = 2f;

    public int numberOfDrop = 2;

    public bool _isAttacking = false;

    void Start()
    {
        StartDroppingPoison();
    }


    public void StartDroppingPoison()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            StartCoroutine(DropPoisonRoutine());
        }
    }

    IEnumerator DropPoisonRoutine()
    {
        for (int i = 0; i < numberOfDrop; i++)
        {
            Vector3 dropPos = transform.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            dropPos.y = 0;
            Instantiate<GameObject>(poisonPrefab, dropPos, Quaternion.identity);
            yield return new WaitForSeconds(dropInterval);
        }
        _isAttacking = false;
    }
}
