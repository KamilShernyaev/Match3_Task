/// <summary>
/// Класс AllServices представляет контейнер для всех сервисов.
/// </summary>
/// <remarks>
/// Он используется для регистрации и получения сервисов в приложении.
/// </remarks>
public class AllServices
{
  private static AllServices _instance;
  public static AllServices Container => _instance ?? (_instance = new AllServices());

  /// <summary>
  /// Метод для регистрации единственного экземпляра сервиса.
  /// </summary>
  /// <typeparam name="TService">Тип сервиса</typeparam>
  /// <param name="implementation">Реализация сервиса</param>
  public void RegisterSingle<TService>(TService implementation) where TService : IService =>
    Implementation<TService>.ServiceInstance = implementation;

  /// <summary>
  /// Метод для получения единственного экземпляра сервиса.
  /// </summary>
  /// <typeparam name="TService">Тип сервиса</typeparam>
  /// <returns>Единственный экземпляр сервиса</returns>
  public TService Single<TService>() where TService : IService =>
    Implementation<TService>.ServiceInstance;

  private class Implementation<TService> where TService : IService
  {
    public static TService ServiceInstance;
  }
}