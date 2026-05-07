namespace CabconMAUI.Helpers;
public static class DlmsHelper
{
    public static class ObisCode
    {
        public static readonly byte[] MeterNumberBytes         ={0x00,0x00,0x60,0x01,0x00,0xFF};
        public static readonly byte[] MeterInfoBytes3PHDLMS    ={0x01,0x00,0x60,0x01,0x01,0xFF};
        public static readonly byte[] BuildVersionBytes        ={0x00,0x00,0x60,0x01,0x04,0xFF};
        public static readonly byte[] InvoCounterPC            ={0x00,0x00,0x2B,0x01,0x00,0xFF};
        public static readonly byte[] InvoCounterMR            ={0x00,0x00,0x2B,0x01,0x02,0xFF};
        public static readonly byte[] InvoCounterUS            ={0x00,0x00,0x2B,0x01,0x03,0xFF};
        public static readonly byte[] InvoCounterFU            ={0x00,0x00,0x2B,0x01,0x05,0xFF};
        public static readonly byte[] Clock                    ={0x00,0x00,0x01,0x00,0x00,0xFF};
        public static readonly byte[] EnergyImport            ={0x01,0x00,0x01,0x08,0x00,0xFF};
        public static readonly byte[] EnergyExport            ={0x01,0x00,0x02,0x08,0x00,0xFF};
        public static readonly byte[] EnergyImportTZ1         ={0x01,0x00,0x01,0x08,0x01,0xFF};
        public static readonly byte[] EnergyImportTZ2         ={0x01,0x00,0x01,0x08,0x02,0xFF};
        public static readonly byte[] EnergyImportTZ3         ={0x01,0x00,0x01,0x08,0x03,0xFF};
        public static readonly byte[] EnergyImportTZ4         ={0x01,0x00,0x01,0x08,0x04,0xFF};
        public static readonly byte[] ReactiveImport           ={0x01,0x00,0x03,0x08,0x00,0xFF};
        public static readonly byte[] ReactiveExport           ={0x01,0x00,0x04,0x08,0x00,0xFF};
        public static readonly byte[] ApparentImport           ={0x01,0x00,0x09,0x08,0x00,0xFF};
        public static readonly byte[] VoltageR                 ={0x01,0x00,0x20,0x07,0x00,0xFF};
        public static readonly byte[] VoltageY                 ={0x01,0x00,0x34,0x07,0x00,0xFF};
        public static readonly byte[] VoltageB                 ={0x01,0x00,0x48,0x07,0x00,0xFF};
        public static readonly byte[] CurrentR                 ={0x01,0x00,0x1F,0x07,0x00,0xFF};
        public static readonly byte[] CurrentY                 ={0x01,0x00,0x33,0x07,0x00,0xFF};
        public static readonly byte[] CurrentB                 ={0x01,0x00,0x47,0x07,0x00,0xFF};
        public static readonly byte[] SignedPowerFactor        ={0x01,0x00,0x0D,0x07,0x00,0xFF};
        public static readonly byte[] Frequency               ={0x01,0x00,0x0E,0x07,0x00,0xFF};
        public static readonly byte[] ActivePowerTotal         ={0x01,0x00,0x01,0x07,0x00,0xFF};
        public static readonly byte[] ReactivePowerTotal       ={0x01,0x00,0x03,0x07,0x00,0xFF};
        public static readonly byte[] BillingProfile           ={0x01,0x00,0x62,0x00,0x00,0xFF};
        public static readonly byte[] LoadProfileInstant       ={0x01,0x00,0x63,0x00,0x00,0xFF};
        public static readonly byte[] TamperProfile            ={0x00,0x00,0x62,0x00,0x00,0xFF};
        public static readonly byte[] DailyProfile             ={0x01,0x00,0x62,0x01,0x00,0xFF};
    }

    public static string[]? DLMSDataFormator(byte[] buf,int idx,bool ascii)
    {
        try{
            if(buf==null||idx>=buf.Length)return null;
            byte dt=buf[idx];
            switch(dt){
                case 0x09: case 0x0A:{
                    int len=buf[idx+1];
                    if(ascii){string s=System.Text.Encoding.ASCII.GetString(buf,idx+2,len).Trim('\0');return new[]{s};}
                    else{string h=BitConverter.ToString(buf,idx+2,len).Replace("-","");return new[]{h};}
                }
                case 0x06:{if(idx+5>buf.Length)return null;uint v=(uint)((buf[idx+1]<<24)|(buf[idx+2]<<16)|(buf[idx+3]<<8)|buf[idx+4]);return new[]{v.ToString()};}
                case 0x05:{if(idx+5>buf.Length)return null;int v=(buf[idx+1]<<24)|(buf[idx+2]<<16)|(buf[idx+3]<<8)|buf[idx+4];return new[]{v.ToString()};}
                case 0x12:{if(idx+3>buf.Length)return null;ushort v=(ushort)((buf[idx+1]<<8)|buf[idx+2]);return new[]{v.ToString()};}
                case 0x10:{if(idx+3>buf.Length)return null;short v=(short)((buf[idx+1]<<8)|buf[idx+2]);return new[]{v.ToString()};}
                case 0x11:return new[]{buf[idx+1].ToString()};
                case 0x0F:return new[]{((sbyte)buf[idx+1]).ToString()};
                case 0x16:return new[]{buf[idx+1].ToString()};
                case 0x0C:{if(idx+13>buf.Length)return null;int yr=(buf[idx+1]<<8)|buf[idx+2];int mo=buf[idx+3];int dy=buf[idx+4];int hr=buf[idx+6];int mn=buf[idx+7];int sc=buf[idx+8];return new[]{$"{dy:D2}-{mo:D2}-{yr} {hr:D2}:{mn:D2}:{sc:D2}"};}
                case 0x01:return new[]{$"Array[{buf[idx+1]}]"};
                case 0x02:return new[]{$"Structure[{buf[idx+1]}]"};
                default:return new[]{$"Type:0x{dt:X2}"};
            }
        }catch{return null;}
    }

    public static long HexToDecimalConversion(string hex){try{return Convert.ToInt64(hex.Replace(" ",""),16);}catch{return 0;}}
    public static string ParseString(byte[] r){if(r==null||r.Length==0)return "N/A";return System.Text.Encoding.ASCII.GetString(r).Trim('\0');}
    public static double ParseDouble(byte[] r){if(r==null||r.Length<8)return 0.0;return BitConverter.ToDouble(r,0);}
    public static DateTime ParseTimestamp(byte[] r){if(r==null||r.Length<8)return DateTime.Now;try{return new DateTime(BitConverter.ToInt64(r,0),DateTimeKind.Utc).ToLocalTime();}catch{return DateTime.Now;}}

    public static List<byte> GetByteByEntryValueType(long from,long to)
    {
        var b=new List<byte>();
        b.Add(0x01);b.Add(0x02);b.Add(0x02);b.Add(0x04);
        b.Add(0x06);b.Add((byte)((from&0xFF000000)>>24));b.Add((byte)((from&0xFF0000)>>16));b.Add((byte)((from&0xFF00)>>8));b.Add((byte)(from&0xFF));
        b.Add(0x06);b.Add((byte)((to&0xFF000000)>>24));b.Add((byte)((to&0xFF0000)>>16));b.Add((byte)((to&0xFF00)>>8));b.Add((byte)(to&0xFF));
        b.Add(0x12);b.Add(0x00);b.Add(0x01);b.Add(0x12);b.Add(0x00);b.Add(0x00);
        return b;
    }

    public static List<byte> GetByteByEntryDateType(DateTime from,DateTime to)
    {
        var b=new List<byte>();
        b.Add(0x01);b.Add(0x01);b.Add(0x02);b.Add(0x04);b.Add(0x02);b.Add(0x04);
        b.Add(0x12);b.Add(0x00);b.Add(0x08);b.Add(0x09);b.Add(0x06);
        b.AddRange(new byte[]{0x00,0x00,0x01,0x00,0x00,0xFF});
        b.Add(0x0F);b.Add(0x02);b.Add(0x12);b.Add(0x00);b.Add(0x00);
        AppendDT(b,from);AppendDT(b,to);b.Add(0x01);b.Add(0x00);
        return b;
    }
    static void AppendDT(List<byte> b,DateTime d){b.Add(0x09);b.Add(0x0C);b.Add((byte)((d.Year&0xFF00)>>8));b.Add((byte)(d.Year&0xFF));b.Add((byte)d.Month);b.Add((byte)d.Day);b.Add(0xFF);b.Add((byte)d.Hour);b.Add((byte)d.Minute);b.Add((byte)d.Second);b.Add(0xFF);b.Add(0x80);b.Add(0x00);b.Add(0x00);}

    public static byte[] GetHexStringToByteArray(string hex){hex=hex.Replace(" ","").Replace("-","");var r=new byte[hex.Length/2];for(int i=0;i<r.Length;i++)r[i]=Convert.ToByte(hex.Substring(i*2,2),16);return r;}
    public static string ByteArrayToHexString(byte[] d,int len)=>BitConverter.ToString(d,0,len).Replace("-"," ");
    public static void FillBerLength(byte[] b,int s,int l){if(l<128)b[s]=(byte)l;else if(l<256){b[s]=0x81;b[s+1]=(byte)l;}else{b[s]=0x82;b[s+1]=(byte)(l>>8);b[s+2]=(byte)l;}}
}
