
public interface IPlayer
{
    public static int Health { get; private set; }
    
    public void TakeDamage(int damage);
    public string GetWeaponName();
}
