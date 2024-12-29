namespace BlApi;

public interface IAdmin
{
    /// <summary>
    /// Sets a predefined maximum range for operations or tasks.
    /// </summary>
    /// <param name="time">The maximum time range to define.</param>
    void Definition(TimeSpan time);

    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The time unit to advance the clock by.</param>
    void ForwardClock(BO.TimeUnit unit);


    DateTime GetClock();

   TimeSpan GetMaxRange();
    //TimeSpan SetMaxRange(TimeSpan maxRange);
    void initialization();

    void Reset();
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

}
