using UnityEngine;

public enum FoodType { Food, MassGainer, MassBurner }

public class Food : MonoBehaviour
{
    [SerializeField] public FoodType foodType;
    [SerializeField] public int sizeChange = 1;
    [SerializeField] public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}