using UnityEngine;

public class TpNextLevel : MonoBehaviour
{
    public LayerMask playerLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayer();
    }

    void CheckPlayer()
    {
        Collider[] target = Physics.OverlapSphere(
            transform.position,
            1,
            playerLayer);
        if (target.Length <= 0) return;
        if (target[0].name == "Tank")
        {
            GameManager.Instance.Portal(transform.position);
        }
    }
}
