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

    /// <summary>
    /// Retrieves the current system clock.
    /// </summary>
    /// <returns>The current date and time.</returns>
    DateTime GetClock();

    /// <summary>
    /// Retrieves the maximum time range that has been defined.
    /// </summary>
    /// <returns>The maximum time range.</returns>
    TimeSpan GetMaxRange();

    /// <summary>
    /// Initializes the system, preparing it for operation.
    /// </summary>
    void initialization();

    /// <summary>
    /// Resets the system to its initial state.
    /// </summary>
    void Reset();
}
