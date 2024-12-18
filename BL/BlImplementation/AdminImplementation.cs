using BlApi;
using DalTest;
using Helpers;


namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Definition(TimeSpan time)
    {
        _dal.ResetDB();
        Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ForwardClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.MINUTE => ClockManager.Now.AddMinutes(1),
        BO.TimeUnit.HOUR => ClockManager.Now.AddHours(1),
        BO.TimeUnit.DAY => ClockManager.Now.AddDays(1),
        BO.TimeUnit.MONTH => ClockManager.Now.AddMonths(1),
        BO.TimeUnit.YEAR => ClockManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });


    public DateTime GetClock() => _dal.Config.Clock;


    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    public void initialization()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void Reset()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}