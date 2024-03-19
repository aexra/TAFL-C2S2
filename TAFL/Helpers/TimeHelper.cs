namespace TAFL.Helpers;
public class TimeHelper
{
    public static DateTime Now => DateTime.Now;

    public static string GetNowString()
    {
        return Now.TimeOfDay.ToString()[..8];
    }
    public static string ToTimeString(DateTime date)
    {
        return ToTimeString(date.TimeOfDay);
    }
    public static string ToTimeString(TimeSpan time)
    {
        var hours = time.Hours;
        var minutes = time.Minutes;
        var seconds = time.Seconds;

        var hours_s = hours.ToString().Length < 2 ? "0" + hours.ToString() : hours.ToString();
        var minutes_s = minutes.ToString().Length < 2 ? "0" + minutes.ToString() : minutes.ToString();
        var seconds_s = seconds.ToString().Length < 2 ? "0" + seconds.ToString() : seconds.ToString();

        return hours_s + ":" + minutes_s + ":" + seconds_s;
    }
}