using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class HdlcFrameService : IHdlcFrameService
{
    private const ushort InitialFcs16 = 0xFFFF;
    private readonly ISerialPortService _serial;
    private static readonly ushort[] uifcstab =
    {
        0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF, 0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
        0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E, 0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
        0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD, 0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
        0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C, 0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
        0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB, 0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
        0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A, 0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
        0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9, 0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
        0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738, 0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
        0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7, 0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
        0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036, 0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
        0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5, 0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
        0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134, 0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
        0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3, 0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
        0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232, 0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
        0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1, 0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
        0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330, 0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
    };

    public HdlcFrameService(ISerialPortService serial)
    {
        _serial = serial;
    }

    public long InitializationCounter{get;set;}=0;
    public int  SecuritysuitByte{get;set;}=0x00;
    private byte _snd=0,_rcv=0,_cmd=0x10;
    public byte nCMDByte=>_cmd;
    private ushort _lastFcs=0;

    public int fAdd7E(byte[] b,int i){b[i++]=0x7E;return i;}
    public int fAddHDLCFrameTag(byte[] b,int i){b[i++]=0xA0;b[i++]=0x00;return i;}
    public int fAddServerSAP(byte[] b,int i,int srv,int mac){
        b[i++]=(byte)((mac>>8)&0x7F);b[i++]=(byte)(mac&0xFE);
        b[i++]=(byte)(((srv<<1)|0x01)&0xFF);return i;}
    public int fAddClientSAP(byte[] b,int i,int cli){b[i++]=(byte)(((cli<<1)|0x01)&0xFF);return i;}
    public void fIncSend(){_snd=(byte)((_snd+1)&0x07);}
    public int fAddCmdByte(byte[] b,int i){_cmd=(byte)((_snd<<1)|(_rcv<<5));b[i++]=_cmd;return i;}
    public int fAddBlankFCS(byte[] b,int i){b[i++]=0;b[i++]=0;return i;}
    public void ffillLength(byte[] b,int idx){int l=Math.Max(0,idx-1);b[1]=(byte)(0xA0|((l>>8)&0x07));b[2]=(byte)(l&0xFF);}
    public void fGenerateFCS(byte[] b,int s,int e){_lastFcs=fPPPfcs16(b,s,e);}
    public void fFillFCS(byte[] b,int p1,int p2){b[p1]=(byte)(_lastFcs&0xFF);b[p2]=(byte)((_lastFcs>>8)&0xFF);}
    public int FillWriteParameters(byte[] b,int i,List<byte> data){foreach(var x in data)b[i++]=x;return i;}
    public bool fCheckStartEndTag(byte[] b)=>b!=null&&b.Length>=4&&b[0]==0x7E&&b[b.Length-1]==0x7E;
    public bool fCheckFCS(byte[] b){try{if(!TryGetHeaderOffsets(b,out var controlIndex,out var hcsIndex,out _,out var fcsIndex))return false;ushort h=fPPPfcs16(b,1,controlIndex);if(b[hcsIndex]!=(byte)(h&0xFF)||b[hcsIndex+1]!=(byte)((h>>8)&0xFF))return false;ushort t=fPPPfcs16(b,1,fcsIndex-1);return b[fcsIndex]==(byte)(t&0xFF)&&b[fcsIndex+1]==(byte)((t>>8)&0xFF);}catch{return false;}}
    public bool fCheckServerSAP(byte[] b,int cli){try{if(!TryGetSourceAddress(b,out var start,out var end))return false;if(end<start)return false;return b[end]==(byte)(((cli<<1)|0x01)&0xFF);}catch{return false;}}
    public bool fCheckCommand(byte[] b,byte e){try{return TryGetControlByte(b,out var control)&&control==(byte)(e|0x01);}catch{return false;}}
    public void fIncRecieve(){_rcv=(byte)((_rcv+1)&0x07);_cmd=(byte)((_snd<<1)|(_rcv<<5)|0x01);}
    public bool fSendSNRM(int srv,int mac,int cli,int cb,int cbR,int w,int wR)
    {
        // STRATEGY: Send a "Pure" SNRM first - just the header without info field
        // Current TX: 7E A0 0B 01 00 03 81 93 00 00 B4 F0 7E (11 bytes payload)
        // Recommended TX: 7E A0 07 03 21 93 0F 01 7E
        var pureSnrm = BuildUnnumberedFrame(srv, mac, cli, 0x93, Array.Empty<byte>());
        
        if (!_serial.fSendDataToPort(pureSnrm, pureSnrm.Length))
        {
            return false;
        }

        // Check for UA response
        var startTime = DateTime.UtcNow;
        while ((DateTime.UtcNow - startTime).TotalMilliseconds < 3000)
        {
            if (_serial.BufferIndex > 0)
            {
                return ValidateUnnumberedResponse(_serial.ReceiveBuffer, _serial.BufferIndex, cli, 0x73);
            }
            System.Threading.Thread.Sleep(50);
        }
        
        // If pure SNRM fails, try full SNRM with parameters as fallback
        Span<byte> snrmInfo = stackalloc byte[]
        {
            0x81, 0x80, 0x12,
            0x05, 0x01, (byte)Math.Clamp(cb, 0x01, 0xFF),
            0x06, 0x01, (byte)Math.Clamp(cbR, 0x01, 0xFF),
            0x07, 0x04, 0x00, 0x00, 0x00, (byte)Math.Clamp(w, 0x01, 0x7F),
            0x08, 0x04, 0x00, 0x00, 0x00, (byte)Math.Clamp(wR, 0x01, 0x7F)
        };
        var fullSnrm = BuildUnnumberedFrame(srv, mac, cli, 0x93, snrmInfo.ToArray());
        if (!_serial.fSendDataToPort(fullSnrm, fullSnrm.Length))
        {
            return false;
        }

        // Wait for UA response with timeout
        startTime = DateTime.UtcNow;
        var timeoutMs = 3000; // 3 second timeout
        
        while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeoutMs)
        {
            if (_serial.BufferIndex > 0)
            {
                return ValidateUnnumberedResponse(_serial.ReceiveBuffer, _serial.BufferIndex, cli, 0x73);
            }
            System.Threading.Thread.Sleep(50); // Check every 50ms
        }
        
        return false; // Timeout - no UA received
    }
    public bool fSendDISC(int srv,int mac,int cli)
    {
        var frame = BuildUnnumberedFrame(srv, mac, cli, 0x53, Array.Empty<byte>());
        if (!_serial.fSendDataToPort(frame, frame.Length))
        {
            return false;
        }

        return ValidateUnnumberedResponse(_serial.ReceiveBuffer, _serial.BufferIndex, cli, 0x73, 0x1F);
    }
    public bool IrDACheckBCC(byte[] b){if(b==null||b.Length<2)return false;byte x=0;for(int i=1;i<b.Length-1;i++)x^=b[i];return x==b[b.Length-1];}
    public bool IrDACheckSyncWord(byte[] b)=>b!=null&&b.Length>=2&&b[0]==0x95&&b[1]==0x95;
    public bool IrDACheckCommandID(byte[] b,byte c)=>b!=null&&b.Length>=7&&b[6]==c;
    public bool IrDACheckBCC_1P(byte[] b,int l){if(b==null||l<1)return false;byte x=0;for(int i=0;i<l-1;i++)x^=b[i];return x==b[l-1];}
    public bool IrDACheckSyncWord_1P(byte[] b,int l)=>b!=null&&l>=2&&b[0]==0x68&&b[1]==0x68;
    public static ushort FCS16(byte[] d,int s,int n)=>fPPPfcs16(d,s,n);

    private static ushort fPPPfcs16(byte[] data,int start,int length)
    {
        ushort fcs = InitialFcs16;
        var end = Math.Min(data.Length, start + length);
        for (var i = start; i < end; i++)
        {
            fcs = (ushort)((fcs >> 8) ^ uifcstab[(fcs ^ data[i]) & 0xFF]);
        }

        return (ushort)~fcs;
    }

    private byte[] BuildUnnumberedFrame(int srv,int mac,int cli,byte control,byte[] information)
    {
        var buffer = new byte[128];
        var index = 0;
        index = fAdd7E(buffer, index);
        index = fAddHDLCFrameTag(buffer, index);
        index = fAddServerSAP(buffer, index, srv, mac);
        index = fAddClientSAP(buffer, index, cli);
        buffer[index++] = control;
        index = fAddBlankFCS(buffer, index);
        if (information.Length > 0)
        {
            Buffer.BlockCopy(information, 0, buffer, index, information.Length);
            index += information.Length;
        }
        index = fAddBlankFCS(buffer, index);
        ffillLength(buffer, index);
        fGenerateFCS(buffer, 1, 8);
        fFillFCS(buffer, 9, 10);
        fGenerateFCS(buffer, 1, index - 3);
        fFillFCS(buffer, index - 2, index - 1);
        index = fAdd7E(buffer, index);
        return buffer[..index];
    }

    private bool ValidateUnnumberedResponse(byte[] frame,int length,int clientSap,params byte[] expectedControls)
    {
        if (length <= 0)
        {
            return false;
        }

        var actual = frame[..Math.Min(length, frame.Length)];
        if (!fCheckStartEndTag(actual) || !fCheckFCS(actual) || !fCheckServerSAP(actual, clientSap))
        {
            return false;
        }

        return TryGetControlByte(actual, out var control) && expectedControls.Contains(control);
    }

    private static bool TryGetControlByte(byte[] frame,out byte control)
    {
        control = 0x00;
        if (!TryGetHeaderOffsets(frame, out var controlIndex, out _, out _, out _))
        {
            return false;
        }

        control = frame[controlIndex];
        return true;
    }

    private static bool TryGetSourceAddress(byte[] frame,out int start,out int end)
    {
        start = 0;
        end = 0;
        if (frame == null || frame.Length < 8)
        {
            return false;
        }

        var index = 3;
        if (!TryAdvanceAddress(frame, ref index))
        {
            return false;
        }

        start = index;
        if (!TryAdvanceAddress(frame, ref index))
        {
            return false;
        }

        end = index - 1;
        return true;
    }

    private static bool TryGetHeaderOffsets(byte[] frame,out int controlIndex,out int hcsIndex,out int infoStart,out int fcsIndex)
    {
        controlIndex = 0;
        hcsIndex = 0;
        infoStart = 0;
        fcsIndex = 0;
        if (frame == null || frame.Length < 10)
        {
            return false;
        }

        var index = 3;
        if (!TryAdvanceAddress(frame, ref index) || !TryAdvanceAddress(frame, ref index))
        {
            return false;
        }

        controlIndex = index;
        hcsIndex = controlIndex + 1;
        infoStart = hcsIndex + 2;
        fcsIndex = frame.Length - 3;
        return fcsIndex > hcsIndex + 1;
    }

    private static bool TryAdvanceAddress(byte[] frame,ref int index)
    {
        while (index < frame.Length - 3)
        {
            if ((frame[index++] & 0x01) == 0x01)
            {
                return true;
            }
        }

        return false;
    }

    public byte[] CreateGetRequest(byte[] obisCode)
    {
        var buffer = new byte[256];
        var index = 0;

        // HDLC Frame Header
        index = fAdd7E(buffer, index);
        index = fAddHDLCFrameTag(buffer, index);
        index = fAddServerSAP(buffer, index, 0x01, 0x8001); // Default server SAP and MAC
        index = fAddClientSAP(buffer, index, 0x10); // Default client SAP
        fIncSend();
        index = fAddCmdByte(buffer, index);
        index = fAddBlankFCS(buffer, index);

        // LLC Layer
        buffer[index++] = 0xE6; // LLC
        buffer[index++] = 0xE6; // LLC
        buffer[index++] = 0x00; // LLC

        // COSEM GET Request
        buffer[index++] = 0xC4; // GET-Request Normal tag
        buffer[index++] = 0x01; // Invoke ID
        buffer[index++] = 0x00; // Priority
        
        // Add OBIS code (class, attribute)
        buffer[index++] = 0x00; // Class (0x00 = Interface class)
        buffer[index++] = 0x01; // Class (0x00 = Interface class)
        buffer[index++] = obisCode[0]; // OBIS Group A
        buffer[index++] = obisCode[1]; // OBIS Group B  
        buffer[index++] = obisCode[2]; // OBIS Group C
        buffer[index++] = obisCode[3]; // OBIS Group D
        buffer[index++] = obisCode[4]; // OBIS Group E
        buffer[index++] = obisCode[5]; // OBIS Group F
        buffer[index++] = 0x01; // Attribute 1 (value)
        buffer[index++] = 0x00; // Access selector (none)

        index = fAddBlankFCS(buffer, index);
        ffillLength(buffer, index);
        fGenerateFCS(buffer, 1, 8);
        fFillFCS(buffer, 9, 10);
        fGenerateFCS(buffer, 1, index - 3);
        fFillFCS(buffer, index - 2, index - 1);
        index = fAdd7E(buffer, index);

        var result = new byte[index];
        Array.Copy(buffer, result, index);
        return result;
    }

    public byte[] CreateGetNextRequest(uint blockIndex)
    {
        var buffer = new byte[256];
        var index = 0;

        // HDLC Frame Header
        index = fAdd7E(buffer, index);
        index = fAddHDLCFrameTag(buffer, index);
        index = fAddServerSAP(buffer, index, 0x01, 0x8001); // Default server SAP and MAC
        index = fAddClientSAP(buffer, index, 0x10); // Default client SAP
        fIncSend();
        index = fAddCmdByte(buffer, index);
        index = fAddBlankFCS(buffer, index);

        // LLC Layer
        buffer[index++] = 0xE6; // LLC
        buffer[index++] = 0xE6; // LLC
        buffer[index++] = 0x00; // LLC

        // COSEM GET-Next Request
        buffer[index++] = 0xC5; // GET-Request with Block tag
        buffer[index++] = 0x01; // Invoke ID
        buffer[index++] = 0x00; // Priority
        
        // Block Number (4 bytes, big-endian)
        buffer[index++] = (byte)((blockIndex >> 24) & 0xFF);
        buffer[index++] = (byte)((blockIndex >> 16) & 0xFF);
        buffer[index++] = (byte)((blockIndex >> 8) & 0xFF);
        buffer[index++] = (byte)(blockIndex & 0xFF);

        index = fAddBlankFCS(buffer, index);
        ffillLength(buffer, index);
        fGenerateFCS(buffer, 1, 8);
        fFillFCS(buffer, 9, 10);
        fGenerateFCS(buffer, 1, index - 3);
        fFillFCS(buffer, index - 2, index - 1);
        index = fAdd7E(buffer, index);

        var result = new byte[index];
        Array.Copy(buffer, result, index);
        return result;
    }
}
