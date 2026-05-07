using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class CosemService : ICosemService
{
    private readonly IHdlcFrameService _hdlc;
    private readonly ISerialPortService _serial;
    private readonly ISettingsService _settings;
    private readonly ICryptoService _crypto;

    public CosemService(IHdlcFrameService hdlc,ISerialPortService serial,ISettingsService settings,ICryptoService crypto)
    {
        _hdlc = hdlc;
        _serial = serial;
        _settings = settings;
        _crypto = crypto;
    }

    public int nBlockIndex{get;set;}=0; public int nBlockNumber{get;set;}=0;
    public int nTotalPacketSize{get;set;}=0; public int nMaxBufferSize{get;set;}=128;
    public string DedKeystr{get;set;}=string.Empty;
    public int fAddLLCByte(byte[] b,int i){b[i++]=0xE6;b[i++]=0xE7;b[i++]=0x00;return i;}
    public int GetQueryReadByClassOBIS(byte[] b,int i,byte att,byte[] obis,byte cls)
    {b[i++]=0xC0;b[i++]=0x01;b[i++]=0x42;b[i++]=(byte)(cls>>8);b[i++]=(byte)(cls&0xFF);foreach(var x in obis)b[i++]=x;b[i++]=att;b[i++]=0x00;return i;}
    public int GetQueryWriteToMeter(List<byte> data,byte[] b,int i,byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] rt)
    {b[i++]=0xC1;b[i++]=0x01;b[i++]=0x42;b[i++]=(byte)(cls>>8);b[i++]=(byte)(cls&0xFF);foreach(var x in obis)b[i++]=x;b[i++]=att;b[i++]=0x00;b[i++]=dt;b[i++]=dl;return i;}
    public int GetQueryWriteMethodToMeter(byte[] b,int i,byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] rt,byte acc)
    {b[i++]=0xC3;b[i++]=0x01;b[i++]=0x42;b[i++]=(byte)(cls>>8);b[i++]=(byte)(cls&0xFF);foreach(var x in obis)b[i++]=x;b[i++]=att;b[i++]=acc;b[i++]=dt;b[i++]=dl;return i;}
    public int GetQueryToWriteBlockToMeter(byte[] b,int i,byte att,byte[] obis,byte cls,byte dt,int dl,byte[] rt)
    {b[i++]=0xC1;b[i++]=0x02;b[i++]=0x42;b[i++]=(byte)(cls>>8);b[i++]=(byte)(cls&0xFF);foreach(var x in obis)b[i++]=x;b[i++]=att;b[i++]=0x00;return i;}
    public int GetQueryToWriteBlockToMeterWithoutAccessSelector(byte[] b,int i,byte att,byte[] obis,byte cls,byte dt,int dl,byte[] rt)
    {b[i++]=0xC3;b[i++]=0x02;b[i++]=0x42;b[i++]=(byte)(cls>>8);b[i++]=(byte)(cls&0xFF);foreach(var x in obis)b[i++]=x;b[i++]=att;return i;}
    public int FillCommandData(byte[] b,int i,List<byte> d){foreach(var x in d)b[i++]=x;return i;}
    public int fGetBlockTransferPacket(byte[] b,int i)
    {b[i++]=0xC0;b[i++]=0x02;b[i++]=0x42;b[i++]=(byte)((nBlockNumber>>24)&0xFF);b[i++]=(byte)((nBlockNumber>>16)&0xFF);b[i++]=(byte)((nBlockNumber>>8)&0xFF);b[i++]=(byte)(nBlockNumber&0xFF);nBlockNumber++;return i;}
    public int fSetBlockTransferPacket(byte[] b,int i,byte[] data,bool blk)
    {int chunk=Math.Min(nMaxBufferSize,data.Length-nBlockIndex);bool last=(nBlockIndex+chunk)>=data.Length;b[i++]=last?(byte)0x01:(byte)0x00;b[i++]=(byte)((nBlockNumber>>24)&0xFF);b[i++]=(byte)((nBlockNumber>>16)&0xFF);b[i++]=(byte)((nBlockNumber>>8)&0xFF);b[i++]=(byte)(nBlockNumber&0xFF);b[i++]=(byte)(chunk>>8);b[i++]=(byte)(chunk&0xFF);for(int j=0;j<chunk;j++)b[i++]=data[nBlockIndex+j];nBlockIndex+=chunk;nBlockNumber++;return i;}
    public int fSetImgBlockTransferPacket(byte[] b,int i,byte[] data,bool blk,object? e)=>fSetBlockTransferPacket(b,i,data,blk);
    public int fActionNormalImgBlockTransferPacket(byte[] b,int i,byte[] data,object? e){foreach(var x in data)b[i++]=x;return i;}
    public int fCheckCOSEMResponseForGet(byte[] b){try{if(b[14]==0xC4&&b[15]==0x01)return b[18]==0x00?0x01:(b[18]==0x09?0x0E:(int)b[18]);return 0x04;}catch{return 0x04;}}
    public int fCheckCOSEMResponseForSet(byte[] b){try{if(b[14]==0xC5&&b[15]==0x01)return b[18]==0x00?0x01:(b[18]==0x03?0x02:0x04);return 0x04;}catch{return 0x04;}}
    public int fCheckCOSEMResponse(byte[] b){try{if(b[14]==0xC4&&b[15]==0x03){bool last=b[18]==0x01;return last?0x01:0x02;}if(b[14]==0xC4&&b[15]==0x01)return b[18]==0x00?0x01:0x05;return 0x04;}catch{return 0x04;}}
    public int fCheckCOSEMResponseForImageBlockSet(byte[] b){try{if(b[14]==0xC5)return b[18] switch{0x00=>0x01,0x03=>0x02,0x01=>0x04,_=>0x03};return 0x04;}catch{return 0x04;}}
    public bool fSendAARQ(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,byte ac)
    {
        try
        {
            var apdu = BuildAarqApdu(sm, pwd, hlsKey, hlsPwd, cb, pdu, ac, false, _settings.ClientSystemTitle, _settings.Securitysuit, _settings.GlobalEncryptionKey, _settings.AuthenticationKey);
            return SendAssociationRequest(srv, mac, cli, apdu);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[COSEM] Plain AARQ failed: {ex.Message}");
            return false;
        }
    }
    public bool fSendAARQ_Cyphered(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,string cst,int ss,string gek,string ak,int dk)
    {
        try
        {
            var apdu = BuildAarqApdu(sm, pwd, hlsKey, hlsPwd, cb, pdu, (byte)ApplicationContext.LogicalModeWithCiphering, true, cst, ss, gek, ak);
            return SendAssociationRequest(srv, mac, cli, apdu);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[COSEM] Ciphered AARQ failed: {ex.Message}");
            return false;
        }
    }

    private bool SendAssociationRequest(int srv,int mac,int cli,byte[] aarqApdu)
    {
        var cmd = new byte[1024];
        var index = 0;
        index = _hdlc.fAdd7E(cmd, index);
        index = _hdlc.fAddHDLCFrameTag(cmd, index);
        index = _hdlc.fAddServerSAP(cmd, index, srv, mac);
        index = _hdlc.fAddClientSAP(cmd, index, cli);
        _hdlc.fIncSend();
        index = _hdlc.fAddCmdByte(cmd, index);
        index = _hdlc.fAddBlankFCS(cmd, index);
        index = fAddLLCByte(cmd, index);
        Buffer.BlockCopy(aarqApdu, 0, cmd, index, aarqApdu.Length);
        index += aarqApdu.Length;
        index = _hdlc.fAddBlankFCS(cmd, index);
        _hdlc.ffillLength(cmd, index);
        _hdlc.fGenerateFCS(cmd, 1, 8);
        _hdlc.fFillFCS(cmd, 9, 10);
        _hdlc.fGenerateFCS(cmd, 1, index - 3);
        _hdlc.fFillFCS(cmd, index - 2, index - 1);
        index = _hdlc.fAdd7E(cmd, index);

        if (!_serial.fSendDataToPort(cmd, index))
        {
            return false;
        }

        _hdlc.fIncRecieve();
        
        // Wait for AARE response with timeout
        var startTime = DateTime.UtcNow;
        var timeoutMs = 5000; // 5 second timeout for association
        
        while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeoutMs)
        {
            if (_serial.BufferIndex > 0)
            {
                return ValidateAareResponse(_serial.ReceiveBuffer, _serial.BufferIndex, cli);
            }
            System.Threading.Thread.Sleep(50); // Check every 50ms
        }
        
        return false; // Timeout - no AARE received
    }

    private bool ValidateAareResponse(byte[] buffer,int length,int clientSap)
    {
        if (length <= 0)
        {
            return false;
        }

        var frame = buffer[..Math.Min(length, buffer.Length)];
        if (!_hdlc.fCheckStartEndTag(frame) || !_hdlc.fCheckFCS(frame) || !_hdlc.fCheckServerSAP(frame, clientSap) || !_hdlc.fCheckCommand(frame, _hdlc.nCMDByte))
        {
            return false;
        }

        var aareIndex = Array.IndexOf(frame, (byte)0x61);
        if (aareIndex < 0 || aareIndex + 7 >= frame.Length)
        {
            return false;
        }

        for (var i = aareIndex; i < frame.Length - 4; i++)
        {
            if (frame[i] == 0xA2 && frame[i + 1] == 0x03 && frame[i + 2] == 0x02 && frame[i + 3] == 0x01)
            {
                return frame[i + 4] == 0x00;
            }
        }

        return false;
    }

    private byte[] BuildAarqApdu(byte securityMechanism,string password,string hlsKey,string hlsPwd,string conformanceBlock,int pduSize,byte applicationContext,bool ciphered,string clientSystemTitle,int securitySuite,string globalEncryptionKey,string authenticationKey)
    {
        var body = new List<byte>();
        body.AddRange(BuildTagged(0xA1, BuildObjectIdentifier(applicationContext)));

        if (ciphered)
        {
            var titleBytes = GetAsciiBytes(clientSystemTitle);
            body.AddRange(BuildTagged(0xA6, BuildTagged(0x04, titleBytes)));
        }

        var authBytes = GetAuthenticationValue(securityMechanism, password, hlsKey, hlsPwd);
        if (securityMechanism != 0x00 && authBytes.Length > 0)
        {
            body.AddRange(BuildTagged(0x8A, new byte[] { 0x07, 0x80 }));
            body.AddRange(BuildTagged(0x8B, new byte[] { 0x60, 0x85, 0x74, 0x05, 0x08, 0x02, securityMechanism }));
            body.AddRange(BuildTagged(0xAC, BuildTagged(0x80, authBytes)));
        }

        var initiateRequest = BuildInitiateRequest(conformanceBlock, pduSize);
        var userInfo = ciphered
            ? _crypto.CreateCipherCommand(initiateRequest, AesGcmCryptoService.HexToBytes(globalEncryptionKey), AesGcmCryptoService.HexToBytes(authenticationKey), GetAsciiBytes(clientSystemTitle), _hdlc.InitializationCounter, (byte)securitySuite)
            : initiateRequest;

        body.AddRange(BuildTagged(0xBE, BuildTagged(0x04, userInfo)));

        var aarq = new List<byte> { 0x60 };
        aarq.AddRange(EncodeLength(body.Count));
        aarq.AddRange(body);
        return aarq.ToArray();
    }

    private static byte[] BuildObjectIdentifier(byte applicationContext)
    {
        return new byte[] { 0x06, 0x07, 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, applicationContext };
    }

    private static byte[] BuildInitiateRequest(string conformanceBlock,int pduSize)
    {
        var conformance = ParseHex(conformanceBlock, 3);
        return new byte[]
        {
            0x01, 0x00, 0x00, 0x00, 0x06, 0x5F, 0x1F, 0x04,
            0x00, conformance[0], conformance[1], conformance[2],
            (byte)(pduSize >> 8), (byte)(pduSize & 0xFF)
        };
    }

    private static byte[] GetAuthenticationValue(byte securityMechanism,string password,string hlsKey,string hlsPwd)
    {
        if (securityMechanism == 0x00)
        {
            return Array.Empty<byte>();
        }

        if (securityMechanism == 0x01)
        {
            return GetAsciiBytes(password);
        }

        var preferred = !string.IsNullOrWhiteSpace(hlsPwd) ? hlsPwd : hlsKey;
        return TryParseHex(preferred, out var bytes) ? bytes : GetAsciiBytes(preferred);
    }

    private static byte[] BuildTagged(byte tag,byte[] value)
    {
        var encoded = new List<byte> { tag };
        encoded.AddRange(EncodeLength(value.Length));
        encoded.AddRange(value);
        return encoded.ToArray();
    }

    private static byte[] EncodeLength(int length)
    {
        return length switch
        {
            < 0x80 => new byte[] { (byte)length },
            <= 0xFF => new byte[] { 0x81, (byte)length },
            _ => new byte[] { 0x82, (byte)(length >> 8), (byte)(length & 0xFF) }
        };
    }

    private static byte[] ParseHex(string value,int expectedBytes)
    {
        if (!TryParseHex(value, out var bytes) || bytes.Length != expectedBytes)
        {
            return expectedBytes == 3 ? new byte[] { 0x00, 0x1C, 0xFF } : Enumerable.Repeat((byte)0x00, expectedBytes).ToArray();
        }

        return bytes;
    }

    private static bool TryParseHex(string value,out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty);
        if (normalized.Length % 2 != 0)
        {
            return false;
        }

        try
        {
            bytes = new byte[normalized.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(normalized.Substring(i * 2, 2), 16);
            }

            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    private static byte[] GetAsciiBytes(string value)
    {
        return System.Text.Encoding.ASCII.GetBytes(value ?? string.Empty);
    }
}
