namespace CabconMAUI.Models;
/// <summary>Mirrors RTC_ISFormat struct exactly. 12-byte DLMS clock object.</summary>
public class RtcData
{
    public byte YearHighByte      { get; set; }
    public byte YearLowByte       { get; set; }
    public byte Month             { get; set; }
    public byte DayOfMonth        { get; set; }
    public byte DayOfWeek         { get; set; } // 1=Mon..7=Sun 0xFF=unspec
    public byte Hour              { get; set; }
    public byte Minute            { get; set; }
    public byte Second            { get; set; }
    public byte Hundreds          { get; set; }
    public byte DeviationHighByte { get; set; }
    public byte DeviationLowByte  { get; set; }
    public byte ClockStatus       { get; set; } // 0x00=normal 0xFF=unspec
    public byte[] ToByteArray() => new[] { YearHighByte,YearLowByte,Month,DayOfMonth,
        DayOfWeek,Hour,Minute,Second,Hundreds,DeviationHighByte,DeviationLowByte,ClockStatus };
    public static byte GetDlmsDayOfWeek(DateTime dt)
        => dt.DayOfWeek == System.DayOfWeek.Sunday ? (byte)7 : (byte)dt.DayOfWeek;
}
