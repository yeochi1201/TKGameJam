[System.Serializable]
public class PlayerStats
{
    public int Hp;
    public float speed;
    public int attackDamage;

    public PlayerStats(int Hp, float speed, int attackDamage)
    {
        this.Hp = Hp;
        this.speed = speed;
        this.attackDamage = attackDamage;
    }
}
