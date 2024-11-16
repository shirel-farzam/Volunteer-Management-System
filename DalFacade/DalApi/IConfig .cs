 namespace DalApi;
using DO;
public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();
}
