[System.Serializable]
public class PlayerStats
{
    public float MaxHp;
    public float Hp;
    public float speed;
    public float attackDamage;

    public PlayerStats(float MaxHp,float Hp, float speed, float attackDamage)
    {
        this.MaxHp=Hp;
        this.Hp = Hp;
        this.speed = speed;
        this.attackDamage = attackDamage;
    }
}
