
public interface IUIFactory : IService
{
    LoseWindow CreateLoseScreen();
    VictoryWindow CreateVictoryScreen();
}
