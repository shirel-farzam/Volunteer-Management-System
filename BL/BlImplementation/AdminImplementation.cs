namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void UpdateClock(TimeUnit unit)
    {
        DateTime newTime = ClockManager.Now;

        switch (unit)
        {
            case TimeUnit.MINUTE:
                newTime = ClockManager.Now.AddMinutes(1);
                break;
            case TimeUnit.HOUR:
                newTime = ClockManager.Now.AddHours(1);
                break;
            case TimeUnit.DAY:
                newTime = ClockManager.Now.AddDays(1);
                break;
            case TimeUnit.MONTH:
                newTime = ClockManager.Now.AddMonths(1);
                break;
            case TimeUnit.YEAR:
                newTime = ClockManager.Now.AddYears(1);
                break;
            default:
                throw new ArgumentException("Invalid TimeUnit value");
        }

        ClockManager.UpdateClock(newTime);
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }
    public void InitializeDatabase()
    {
        ResetDatabase();
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDatabase()
    {
        _dal.ResetDB();
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange = maxRange;
    }

    public void AdvanceClock(TimeUnit timeUnit)
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetRiskTimeRange()
    {
        throw new NotImplementedException();
    }

    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        throw new NotImplementedException();
    }

}