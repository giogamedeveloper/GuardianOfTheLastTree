using UnityEngine;

public class MineManagerBoss : MonoBehaviour
{
    public static MineManagerBoss Instance { get; private set; }

    public GameObject minePrefab; // Asigna en el inspector el prefab de la mina

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para lanzar minas en posiciones específicas
    public void LaunchMines(Vector3[] positions, float detonateTime)
    {
        foreach (var pos in positions)
        {
            GameObject minaGO = Instantiate(minePrefab, pos, Quaternion.identity);
            MineBoss mina = minaGO.GetComponent<MineBoss>();
            if (mina != null)
            {
                mina.Initialize(); // prepara la mina (por ejemplo, carga referencias)
                mina.DetonateAfter(detonateTime);
            }
        }
    }
}
