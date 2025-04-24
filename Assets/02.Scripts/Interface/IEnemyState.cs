public interface IEnemyState
{
    public void Enter(Enemy enemy);
    public void Execute(Enemy enemy);
    public void Exit(Enemy enemy);
}
