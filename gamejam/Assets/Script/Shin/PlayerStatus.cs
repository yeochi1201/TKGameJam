[System.Serializable]
public class PlayerStats
{
    public float Hp;
    public float speed;
    public float attackDamage;

    public PlayerStats(float Hp, float speed, float attackDamage)
    {
        this.Hp = Hp;
        this.speed = speed;
        this.attackDamage = attackDamage;
    }
}
