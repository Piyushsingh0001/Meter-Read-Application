namespace CabconMAUI.Models;
public class MeterVariant
{
    public int           Id   { get; set; }
    public string        Name { get; set; } = string.Empty;
    public MeterTypeInfo Type { get; set; }
    public override string ToString() => Name;
    public static IReadOnlyList<MeterVariant> VisibleVariants => new List<MeterVariant>
    {
        new(){Id=0,Name="1Phase-Smart Meter",  Type=MeterTypeInfo.Smart_Meter_1PH},
        new(){Id=1,Name="1Phase-DLMS",         Type=MeterTypeInfo.MicroStar_DLMS},
        new(){Id=2,Name="3Phase-Smart Meter",  Type=MeterTypeInfo.Smart_Meter_3PH},
    };
    public static IReadOnlyList<MeterVariant> AllVariants => new List<MeterVariant>
    {
        new(){Id=0,Name="1Phase-Smart Meter",  Type=MeterTypeInfo.Smart_Meter_1PH},
        new(){Id=1,Name="1Phase-DLMS",         Type=MeterTypeInfo.MicroStar_DLMS},
        new(){Id=2,Name="3Phase-Smart Meter",  Type=MeterTypeInfo.Smart_Meter_3PH},
        new(){Id=3,Name="3Phase-DLMS-PUMA",    Type=MeterTypeInfo.DLMS_3PH},
        new(){Id=4,Name="3Phase-Sapphire",     Type=MeterTypeInfo.SAPPHIRE},
        new(){Id=5,Name="3Phase-RUBY",         Type=MeterTypeInfo.DLMS_3PH_RUBY},
        new(){Id=6,Name="1Phase-NON-DLMS",     Type=MeterTypeInfo.Non_DLMS_1PH},
        new(){Id=7,Name="3Phase-Sapphire-S2",  Type=MeterTypeInfo.SAPPHIRE_S2},
    };
    public static Dictionary<string,int> GetMeterTypeCode() => new()
    {
        {"HM",(int)MeterTypeInfo.DLMS_3PH},{"HK",(int)MeterTypeInfo.DLMS_3PH},
        {"LT",(int)MeterTypeInfo.DLMS_3PH},{"WC",(int)MeterTypeInfo.DLMS_3PH},
        {"LC",(int)MeterTypeInfo.DLMS_3PH},{"HC",(int)MeterTypeInfo.DLMS_3PH},
        {"UK",(int)MeterTypeInfo.DLMS_3PH},{"WB",(int)MeterTypeInfo.DLMS_3PH},
        {"BW",(int)MeterTypeInfo.DLMS_3PH},{"uk",(int)MeterTypeInfo.DLMS_3PH},
        {"Ht",(int)MeterTypeInfo.DLMS_3PH},
        {"SC",(int)MeterTypeInfo.SAPPHIRE},{"ST",(int)MeterTypeInfo.SAPPHIRE},
        {"W0",(int)MeterTypeInfo.SAPPHIRE},{"L0",(int)MeterTypeInfo.SAPPHIRE},
        {"SM",(int)MeterTypeInfo.SAPPHIRE},{"SH",(int)MeterTypeInfo.SAPPHIRE},
        {"sm",(int)MeterTypeInfo.SAPPHIRE},{"sh",(int)MeterTypeInfo.SAPPHIRE},
        {"TN",(int)MeterTypeInfo.SAPPHIRE},
        {"LGC110",(int)MeterTypeInfo.MicroStar_DLMS},{"SK",(int)MeterTypeInfo.MicroStar_DLMS},
        {"SF",(int)MeterTypeInfo.MicroStar_DLMS},{"VB",(int)MeterTypeInfo.MicroStar_DLMS},
        {"VF",(int)MeterTypeInfo.MicroStar_DLMS},{"BF",(int)MeterTypeInfo.MicroStar_DLMS},
        {"BK",(int)MeterTypeInfo.MicroStar_DLMS},{"CF",(int)MeterTypeInfo.MicroStar_DLMS},
        {"RF",(int)MeterTypeInfo.MicroStar_DLMS},{"CB",(int)MeterTypeInfo.MicroStar_DLMS},
        {"SM_110",(int)MeterTypeInfo.Smart_Meter_1PH},{"FS",(int)MeterTypeInfo.Smart_Meter_1PH},
        {"SM0110",(int)MeterTypeInfo.Smart_Meter_1PH},
        {"SM_310",(int)MeterTypeInfo.Smart_Meter_3PH},{"SM_405",(int)MeterTypeInfo.Smart_Meter_3PH},
        {"SM0405",(int)MeterTypeInfo.Smart_Meter_3PH},{"SM0310",(int)MeterTypeInfo.Smart_Meter_3PH},
        {"FU",(int)MeterTypeInfo.Smart_Meter_3PH},{"FL",(int)MeterTypeInfo.Smart_Meter_3PH},
        {"FH",(int)MeterTypeInfo.Smart_Meter_3PH},
        {"SPS201",(int)MeterTypeInfo.SAPPHIRE_S2},{"SPS202",(int)MeterTypeInfo.SAPPHIRE_S2},
        {"CC",(int)MeterTypeInfo.MicroStar_DLMS},
    };
}
