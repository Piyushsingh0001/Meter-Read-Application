<<<<<<< HEAD
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;

public class CosemService : ICosemService
{
    private readonly IHdlcFrameService _hdlc;
    private readonly ISerialPortService _serial;
    private readonly ISettingsService _settings;
    private readonly ICryptoService _crypto;

    public CosemService(IHdlcFrameService hdlc, ISerialPortService serial, ISettingsService settings, ICryptoService crypto)
    {
        _hdlc = hdlc;
        _serial = serial;
        _settings = settings;
        _crypto = crypto;
    }

    public int nBlockIndex { get; set; } = 0; public int nBlockNumber { get; set; } = 0;
    public int nTotalPacketSize { get; set; } = 0; public int nMaxBufferSize { get; set; } = 128;
    public string DedKeystr { get; set; } = string.Empty;
    public string LastError { get; set; } = string.Empty;
    public List<byte> BlockBuffer { get; } = new();
    public int fAddLLCByte(byte[] b, int i) { b[i++] = 0xE6; b[i++] = 0xE6; b[i++] = 0x00; return i; }
    public int GetQueryReadByClassOBIS(byte[] b, int i, byte att, byte[] obis, byte cls)
    { b[i++] = 0xC0; b[i++] = 0x01; b[i++] = 0x42; b[i++] = (byte)(cls >> 8); b[i++] = (byte)(cls & 0xFF); foreach (var x in obis) b[i++] = x; b[i++] = att; b[i++] = 0x00; return i; }
    public int GetQueryWriteToMeter(List<byte> data, byte[] b, int i, byte att, byte[] obis, byte cls, byte dt, byte dl, byte[] rt)
    { b[i++] = 0xC1; b[i++] = 0x01; b[i++] = 0x42; b[i++] = (byte)(cls >> 8); b[i++] = (byte)(cls & 0xFF); foreach (var x in obis) b[i++] = x; b[i++] = att; b[i++] = 0x00; b[i++] = dt; b[i++] = dl; return i; }
    public int GetQueryWriteMethodToMeter(byte[] b, int i, byte att, byte[] obis, byte cls, byte dt, byte dl, byte[] rt, byte acc)
    { b[i++] = 0xC3; b[i++] = 0x01; b[i++] = 0x42; b[i++] = (byte)(cls >> 8); b[i++] = (byte)(cls & 0xFF); foreach (var x in obis) b[i++] = x; b[i++] = att; b[i++] = acc; b[i++] = dt; b[i++] = dl; return i; }
    public int GetQueryToWriteBlockToMeter(byte[] b, int i, byte att, byte[] obis, byte cls, byte dt, int dl, byte[] rt)
    { b[i++] = 0xC1; b[i++] = 0x02; b[i++] = 0x42; b[i++] = (byte)(cls >> 8); b[i++] = (byte)(cls & 0xFF); foreach (var x in obis) b[i++] = x; b[i++] = att; b[i++] = 0x00; return i; }
    public int GetQueryToWriteBlockToMeterWithoutAccessSelector(byte[] b, int i, byte att, byte[] obis, byte cls, byte dt, int dl, byte[] rt)
    { b[i++] = 0xC3; b[i++] = 0x02; b[i++] = 0x42; b[i++] = (byte)(cls >> 8); b[i++] = (byte)(cls & 0xFF); foreach (var x in obis) b[i++] = x; b[i++] = att; return i; }
    public int FillCommandData(byte[] b, int i, List<byte> d) { foreach (var x in d) b[i++] = x; return i; }
    public int fGetBlockTransferPacket(byte[] b, int i)
    { b[i++] = 0xC0; b[i++] = 0x02; b[i++] = 0x42; b[i++] = (byte)((nBlockNumber >> 24) & 0xFF); b[i++] = (byte)((nBlockNumber >> 16) & 0xFF); b[i++] = (byte)((nBlockNumber >> 8) & 0xFF); b[i++] = (byte)(nBlockNumber & 0xFF); nBlockNumber++; return i; }
    public int fSetBlockTransferPacket(byte[] b, int i, byte[] data, bool blk)
    { int chunk = Math.Min(nMaxBufferSize, data.Length - nBlockIndex); bool last = (nBlockIndex + chunk) >= data.Length; b[i++] = last ? (byte)0x01 : (byte)0x00; b[i++] = (byte)((nBlockNumber >> 24) & 0xFF); b[i++] = (byte)((nBlockNumber >> 16) & 0xFF); b[i++] = (byte)((nBlockNumber >> 8) & 0xFF); b[i++] = (byte)(nBlockNumber & 0xFF); b[i++] = (byte)(chunk >> 8); b[i++] = (byte)(chunk & 0xFF); for (int j = 0; j < chunk; j++) b[i++] = data[nBlockIndex + j]; nBlockIndex += chunk; nBlockNumber++; return i; }
    public int fSetImgBlockTransferPacket(byte[] b, int i, byte[] data, bool blk, object? e) => fSetBlockTransferPacket(b, i, data, blk);
    public int fActionNormalImgBlockTransferPacket(byte[] b, int i, byte[] data, object? e) { foreach (var x in data) b[i++] = x; return i; }
    public int fCheckCOSEMResponseForGet(byte[] b) { try { if (b[14] == 0xC4 && b[15] == 0x01) return b[18] == 0x00 ? 0x01 : (b[18] == 0x09 ? 0x0E : (int)b[18]); return 0x04; } catch { return 0x04; } }
    public int fCheckCOSEMResponseForSet(byte[] b) { try { if (b[14] == 0xC5 && b[15] == 0x01) return b[18] == 0x00 ? 0x01 : (b[18] == 0x03 ? 0x02 : 0x04); return 0x04; } catch { return 0x04; } }
    public int fCheckCOSEMResponse(byte[] b)
    {
        try
        {
            int idx = 14;
            if (b[idx] != 0xC4) return 0x03; // Tag Mismatch
            idx++;
            if (b[idx] == 0x02) // Get.response.Next (With-DataBlock)
            {
                idx += 2;
                if (b[idx] == 0x00) // Not Last Block
                {
                    idx += 3;
                    nBlockNumber = (b[idx] << 8) | b[idx + 1];
                    idx += 3; // Advance to Length byte (skip Choice byte at b[22])
                    int chunkLen = b[idx];
                    if (b[idx] == 0x82) { chunkLen = ((b[idx + 1] & 0x1F) << 8) | b[idx + 2]; idx += 2; }
                    else if (b[idx] == 0x81) { chunkLen = b[idx + 1]; idx += 1; }
                    idx++;
                    for (int i = 0; i < chunkLen; i++) BlockBuffer.Add(b[idx++]);
                    return 0x02; // More blocks
                }
                else // Last Block
                {
                    idx += 6;
                    int chunkLen = b[idx];
                    if (b[idx] == 0x82) { chunkLen = ((b[idx + 1] & 0x1F) << 8) | b[idx + 2]; idx += 2; }
                    else if (b[idx] == 0x81) { chunkLen = b[idx + 1]; idx += 1; }
                    idx++;
                    for (int i = 0; i < chunkLen; i++) BlockBuffer.Add(b[idx++]);
                    return 0x01; // Success
                }
            }
            else // Get.response.Normal
            {
                idx += 2;
                if (b[idx] == 0x00) // Success
                {
                    idx++; // Skip the 0x00 Data Choice byte
                    
                    int startIdx = 0;
                    while (startIdx < b.Length - 2 && b[startIdx] != 0x7E) startIdx++;
                    if (startIdx < b.Length - 2 && (b[startIdx + 1] & 0xF0) == 0xA0)
                    {
                        int frameLen = ((b[startIdx + 1] & 0x07) << 8) | b[startIdx + 2];
                        int endIdx = startIdx + 1 + frameLen - 2; // Exclude 2 bytes FCS
                        while (idx < endIdx && idx < b.Length)
                        {
                            BlockBuffer.Add(b[idx++]);
                        }
                    }
                    return 0x01; // Success
                }
                return 0x05; // Access Denied
            }
        }
        catch { return 0x04; }
    }
    public int fCheckCOSEMResponseForImageBlockSet(byte[] b) { try { if (b[14] == 0xC5) return b[18] switch { 0x00 => 0x01, 0x03 => 0x02, 0x01 => 0x04, _ => 0x03 }; return 0x04; } catch { return 0x04; } }
    public bool fSendAARQ(int srv, int mac, int cli, byte sm, string pwd, string hlsKey, string hlsPwd, string cb, int pdu, byte ac)
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
    public bool fSendAARQ_Cyphered(int srv, int mac, int cli, byte sm, string pwd, string hlsKey, string hlsPwd, string cb, int pdu, string cst, int ss, string gek, string ak, int dk)
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

    private bool SendAssociationRequest(int srv, int mac, int cli, byte[] aarqApdu)

    {

        System.Diagnostics.Debug.WriteLine($"FORCED Client SAP = {cli}");
        var cmd = new byte[1024];
        var idx = 0;
        idx = _hdlc.fAdd7E(cmd, idx);
        idx = _hdlc.fAddHDLCFrameTag(cmd, idx);
        idx = _hdlc.fAddServerSAP(cmd, idx, srv, mac);
        idx = _hdlc.fAddClientSAP(cmd, idx, cli);
        _hdlc.fSetInitialI();
        idx = _hdlc.fAddCmdByte(cmd, idx);
        idx = _hdlc.fAddBlankFCS(cmd, idx);
        idx = fAddLLCByte(cmd, idx);

        Array.Copy(aarqApdu, 0, cmd, idx, aarqApdu.Length);
        idx += aarqApdu.Length;
        // Reserve 2 bytes for trailing frame FCS (same as HdlcFrameService pattern)
        idx = _hdlc.fAddBlankFCS(cmd, idx);

        _hdlc.ffillLength(cmd, idx);
        _hdlc.fGenerateFCS(cmd, 1, 8);       // Header FCS over bytes[1..8]
        _hdlc.fFillFCS(cmd, 9, 10);           // Write header FCS at bytes[9,10]
        _hdlc.fGenerateFCS(cmd, 1, idx - 3);  // Frame FCS: bytes[1] to just before the 2 FCS bytes + 7E
        _hdlc.fFillFCS(cmd, idx - 2, idx - 1);// Write frame FCS at the reserved positions
        idx = _hdlc.fAdd7E(cmd, idx);

        System.Diagnostics.Debug.WriteLine($"[DLMS] AARQ Tx: {BitConverter.ToString(cmd, 0, idx)}");
        LastError = $"AARQ Tx: {BitConverter.ToString(cmd, 0, idx)}";

        if (!_serial.fSendDataToPort(cmd, idx))
        {
            return false;
        }
        System.Diagnostics.Debug.WriteLine($"[DLMS] AARQ Tx: {BitConverter.ToString(cmd, 0, idx)}");
        System.Diagnostics.Debug.WriteLine($"Client SAP = {cli}");
        _hdlc.fIncRecieve();

        return ValidateAareResponse(_serial.ReceiveBuffer, _serial.BufferIndex, cli);

    }

    private bool ValidateAareResponse(byte[] buffer, int length, int clientSap)
    {
        if (length <= 0)
        {
            return false;
        }

        var frame = buffer[..Math.Min(length, buffer.Length)];
        LastError = $"AARE Raw Rx: {BitConverter.ToString(frame)}";
        System.Diagnostics.Debug.WriteLine($"[DLMS] {LastError}");

        if (!_hdlc.fCheckStartEndTag(frame) || !_hdlc.fCheckFCS(frame) || !_hdlc.fCheckServerSAP(frame, clientSap))
        {
            LastError += " | Error: StartEnd/FCS/SAP validation failed.";
            return false;
        }
        if (!_hdlc.fCheckCommand(frame, _hdlc.nCMDByte))
        {
            LastError += $" | Error: CMD failed. Exp: {_hdlc.nCMDByte:X2}, Got: {frame[8]:X2}";
            return false;
        }

        // Just check for A2 03 02 01 00 like Desktop does!
        for (int i = 0; i < frame.Length - 4; i++)
        {
            if (frame[i] == 0xA2 && frame[i + 1] == 0x03 && frame[i + 2] == 0x02 && frame[i + 3] == 0x01)
            {
                if (frame[i + 4] == 0x00)
                {
                    LastError += " | AARE OK.";
                    return true;
                }
                else
                {
                    LastError += $" | AARE Rejected by meter. Reason: {frame[i + 4]:X2}";
                    return false;
                }
            }
        }

        LastError += " | Error: AARE Result Tag (A2 03 02 01 00) not found in frame.";
        return false;


    }

    private byte[] BuildAarqApdu(byte securityMechanism, string password, string hlsKey, string hlsPwd, string conformanceBlock, int pduSize, byte applicationContext, bool ciphered, string clientSystemTitle, int securitySuite, string globalEncryptionKey, string authenticationKey)
    {
        var body = new List<byte>();
        body.AddRange(BuildTagged(0xA1, BuildObjectIdentifier(applicationContext)));

        if (ciphered)
        {
            var titleBytes = GetAsciiBytes(clientSystemTitle);
            body.AddRange(BuildTagged(0xA6, BuildTagged(0x04, titleBytes)));
        }

        // Removed hardcoded override that forced Low Auth:
        // if (securityMechanism == 0x00) securityMechanism = 0x01; // Force Low Auth for Smart Meter
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

    private static byte[] BuildInitiateRequest(string conformanceBlock, int pduSize)
    {
        var conformance = ParseHex(conformanceBlock, 3);
        return new byte[]
        {
            0x01, 0x00, 0x00, 0x00, 0x06, 0x5F, 0x1F, 0x04,
            0x00, conformance[0], conformance[1], conformance[2],
            (byte)(pduSize >> 8), (byte)(pduSize & 0xFF)
        };
    }

    private static byte[] GetAuthenticationValue(byte securityMechanism, string password, string hlsKey, string hlsPwd)
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

    private static byte[] BuildTagged(byte tag, byte[] value)
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

    private static byte[] ParseHex(string value, int expectedBytes)
    {
        if (!TryParseHex(value, out var bytes))
        {
            return expectedBytes == 3 ? new byte[] { 0x00, 0x1C, 0xFF } : Enumerable.Repeat((byte)0x00, expectedBytes).ToArray();
        }

        if (bytes.Length >= expectedBytes)
        {
            var truncated = new byte[expectedBytes];
            Array.Copy(bytes, truncated, expectedBytes);
            return truncated;
        }

        var padded = new byte[expectedBytes];
        Array.Copy(bytes, padded, bytes.Length);
        return padded;
    }

    private static bool TryParseHex(string value, out byte[] bytes)
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
=======
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class CosemService : ICosemService
{
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
    public bool fSendAARQ(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,byte ac){System.Diagnostics.Debug.WriteLine("[COSEM] fSendAARQ (stub)");return true;}
    public bool fSendAARQ_Cyphered(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,string cst,int ss,string gek,string ak,int dk){System.Diagnostics.Debug.WriteLine("[COSEM] fSendAARQ_Cyphered (stub)");return true;}
>>>>>>> bf49aa6198f381896113163ead8b83e92944c023
}
