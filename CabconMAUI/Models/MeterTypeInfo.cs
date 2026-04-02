namespace CabconMAUI.Models;
public enum MeterTypeInfo { Smart_Meter_1PH=0, MicroStar_DLMS=1, Smart_Meter_3PH=2, DLMS_3PH=3, SAPPHIRE=4, DLMS_3PH_RUBY=5, Non_DLMS_1PH=6, SAPPHIRE_S2=7 }
public enum ApplicationContext : byte { ShortMode=2, LogicalModeWithoutCiphering=1, LogicalModeWithCiphering=3 }
public enum ProgrammingCode { Success, Fail, AccessDenied, DataUnavailable, TimeOut, SignOnFailed, CosemConnectionFailed, MeterIDMismatch }
public enum DisplayProgrammingTypes { OneByte=0, TwoByte=1 }
public enum IECSignOnMode { IEC_READ=0, IEC_MANUFACURER=1, IEC_PRGRAMING=2 }
