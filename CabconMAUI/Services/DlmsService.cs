using CabconMAUI.Helpers;
using CabconMAUI.Models;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class DlmsService : IDlmsService
{
    readonly ISettingsService  _set;
    readonly ISerialPortService _ser;
    readonly IHdlcFrameService  _hdlc;
    readonly ICosemService      _cosem;
    readonly ICryptoService     _crypto;
    readonly byte[] _cmd=new byte[1024];
    int _idx=0;
    string _llsPwd="",_hlsPwd="",_encKey="",_tmpKey="";
    public bool IsMeterConnected{get;private set;}
    public string MeterInfoValue{get;private set;}=string.Empty;
    public string CurrentMeterID{get;private set;}=string.Empty;
    public string AESstr{get;private set;}=string.Empty;
    public int GetWriteResponseCode{get;private set;}
    public List<string>? ListSecurityKeys{get;private set;}
    public string RSAPrivateKey{get;set;}=string.Empty;
    public static string MeterSignature{get;private set;}=string.Empty;
    public event EventHandler<StatusEventArgs>? StatusUpdated;
    public DlmsService(ISettingsService s,ISerialPortService ser,IHdlcFrameService hdlc,ICosemService cosem,ICryptoService crypto){_set=s;_ser=ser;_hdlc=hdlc;_cosem=cosem;_crypto=crypto;}
    void Raise(string m,bool e){System.Diagnostics.Debug.WriteLine($"[DLMS][{(e?"E":"I")}] {m}");MainThread.BeginInvokeOnMainThread(()=>StatusUpdated?.Invoke(this,new StatusEventArgs(m,e)));}

    public async Task<bool> ConnectToMeterAsync()
    {
        CurrentMeterID=string.Empty;
        if(_set.ApplicationContext==(byte)ApplicationContext.LogicalModeWithCiphering){
            if(!await ReadAssociationForInvocationCounterAsync())return false;
            if(CurrentMeterID.Trim().Length<=0){Raise("Invalid Meter ID!",true);return false;}
            if(_set.GetAssociationMode()==0)_set.SetCipheredSecurityResponse(_llsPwd,_hlsPwd,_encKey);
        }
        IsMeterConnected=false;MeterInfoValue=string.Empty;
        Raise("  Physical Layer Communication...",false);
        if(!await PhysicalLayerConnectAsync()){Raise("Physical Layer Connection Failed!",true);return false;}
        Raise("HDLC Layer Communication...",false);
        if(!await HDLCLayerConnectAsync()){Raise("HDLC Layer Connection Failed/ Busy !",true);return false;}
        Raise("Establishing Association...",false);
        IsMeterConnected=true;
        if(!await AssociationStablishAsync()){
            if(!string.IsNullOrEmpty(_tmpKey)&&_tmpKey.Length>1){
                _set.SetCipheredSecurityResponse(_llsPwd,_hlsPwd,_tmpKey);
                if(!await PhysicalLayerConnectAsync()){Raise("Physical Layer Connection Failed!",true);return false;}
                if(!await HDLCLayerConnectAsync()){Raise("HDLC Layer Connection Failed/ Busy !",true);return false;}
                if(!await AssociationStablishAsync()){Raise("Unable To Establish Association!",true);return false;}
                _encKey=_tmpKey;
            }else{Raise("Unable To Establish Association!",true);return false;}
        }
        if(!string.IsNullOrEmpty(_cosem.DedKeystr))_hdlc.InitializationCounter=0;
        Raise("Checking Meter Type Info...",false);
        string csh=Convert.ToInt32(_set.GetClientSAP(),10).ToString("X");
        if(csh!="10"&&!await ValidMeterTypeInfoAsync()){Raise("Invalid Meter Detected, Check Meter Variant !",true);return false;}
        Raise("Data Transferring Please Wait...",false);
        return true;
    }
    public async Task<bool> PhysicalLayerConnectAsync()=>await Task.Run(()=>{try{_ser.SetSerialPortSettings(_set.SerialPort,_set.CommandBaudRate,"None","8","1",_set.CommandTimeOut,_set.IntercharacterDelay);return _ser.OpenPort();}catch{return false;}});
    public void PhysicalLayerDisconnect(){try{_ser.ClosePort();}catch{}}
    public async Task<bool> HDLCLayerConnectAsync()=>await Task.Run(()=>{try{return _hdlc.fSendSNRM(_set.ServerSAP,_set.ServerLowerMacAddress,_set.ClientSAP,_set.CosemBufferSize,_set.CosemBufferSize,_set.WindowSize,_set.WindowSize);}catch{return false;}});
    public async Task<bool> AssociationStablishAsync()=>await Task.Run(()=>{try{if(_set.ApplicationContext==(byte)ApplicationContext.LogicalModeWithCiphering){AESstr="Cyphering";return _cosem.fSendAARQ_Cyphered(_set.ServerSAP,_set.ServerLowerMacAddress,_set.ClientSAP,_set.SecurityMechanism,_set.Password,_set.HLSKey,_set.HLSPWD,_set.ConformanceBlock,_set.PDUSize,_set.ClientSystemTitle,_set.Securitysuit,_set.GlobalEncryptionKey,_set.AuthenticationKey,_set.DedicatedKey);}else return _cosem.fSendAARQ(_set.ServerSAP,_set.ServerLowerMacAddress,_set.ClientSAP,_set.SecurityMechanism,_set.Password,_set.HLSKey,_set.HLSPWD,_set.ConformanceBlock,_set.PDUSize,_set.ApplicationContext);}catch{return false;}});
    public async Task<bool> ReadAssociationForInvocationCounterAsync(){
        byte osm=_set.GetSecurityMachanism();string ocs=_set.GetClientSAP();byte oac=_set.GetApplicationContext();
        try{IsMeterConnected=false;MeterInfoValue=string.Empty;_set.SetSecurityMachanism(0x00);_set.SetClientSAP(0x10);_set.SetApplicationContext(0x01);
            Raise("Physical Layer Communication...",false);if(!await PhysicalLayerConnectAsync()){Raise("Physical Layer Connection Failed!",true);return false;}
            Raise("HDLC Layer Communication...",false);if(!await HDLCLayerConnectAsync()){Raise("HDLC Layer Connection Failed/ Busy !",true);return false;}
            IsMeterConnected=true;_cosem.fSendAARQ(_set.ServerSAP,_set.ServerLowerMacAddress,_set.ClientSAP,_set.SecurityMechanism,_set.Password,_set.HLSKey,_set.HLSPWD,_set.ConformanceBlock,_set.PDUSize,_set.ApplicationContext);
            int resp=await ReadDataCommandAsync(DlmsHelper.ObisCode.MeterNumberBytes,0x01,0x02);
            string[]? dv=DlmsHelper.DLMSDataFormator(_ser.ReceiveBuffer,18,true);if(dv!=null&&dv.Length>0)CurrentMeterID=dv[0];
            byte[] invoObis=ocs switch{"32"=>new byte[]{0x00,0x00,0x2B,0x01,0x02,0xFF},"48"=>new byte[]{0x00,0x00,0x2B,0x01,0x03,0xFF},"80"=>new byte[]{0x00,0x00,0x2B,0x01,0x05,0xFF},_=>new byte[]{0x00,0x00,0x2B,0x01,0x00,0xFF}};
            await ReadDataCommandAsync(invoObis,0x01,0x02);string[]? ic=DlmsHelper.DLMSDataFormator(_ser.ReceiveBuffer,18,false);long icv=0;if(ic!=null&&ic.Length>0)icv=Convert.ToInt64(ic[0]);_hdlc.InitializationCounter=icv+1;
            await AssociationDisconnectAsync();return true;}
        catch{return false;}finally{_set.SetSecurityMachanism(osm);_set.SetClientSAP(Convert.ToInt16(ocs));_set.SetApplicationContext(oac);}
    }
    public async Task<bool> AssociationDisconnectAsync(){try{if(!IsMeterConnected)return true;bool ok=await Task.Run(()=>_hdlc.fSendDISC(_set.ServerSAP,_set.ServerLowerMacAddress,_set.ClientSAP));if(!ok)Raise("Unable To Close Current Association!",true);return ok;}catch{Raise("Unable To Close Current Association!",true);return false;}finally{PhysicalLayerDisconnect();}}
    public async Task<bool> ValidMeterTypeInfoAsync(){try{if(_set.MeterMode==(int)MeterTypeInfo.DLMS_3PH_RUBY)return true;byte[] obis=(_set.MeterMode==(int)MeterTypeInfo.Smart_Meter_1PH||_set.MeterMode==(int)MeterTypeInfo.Smart_Meter_3PH||_set.MeterMode==(int)MeterTypeInfo.SAPPHIRE_S2)?DlmsHelper.ObisCode.BuildVersionBytes:DlmsHelper.ObisCode.MeterInfoBytes3PHDLMS;if(!await ReadByteFromMeterAsync(obis,0x01,0x02)){if(GetWriteResponseCode==(int)ProgrammingCode.AccessDenied)return true;return false;}return IsValidMeterType(_ser.ReceiveBuffer,_set.MeterMode);}catch{return false;}}
    bool IsValidMeterType(byte[] d,int mode){try{int si=18,startI=15,endI=2;if(mode==(int)MeterTypeInfo.Smart_Meter_1PH||mode==(int)MeterTypeInfo.Smart_Meter_3PH||mode==(int)MeterTypeInfo.SAPPHIRE_S2){startI=4;endI=6;}else if(d[si+1]>=0x3F){startI=17;endI=2;}else if(d[si+1]>=0x1E){startI=16;endI=2;}if(d[si]==0x09||d[si]==0x0A){string[]? v=DlmsHelper.DLMSDataFormator(d,si,true);if(v!=null&&v.Length>=1)MeterSignature=MeterInfoValue=v[0];}else return true;if(MeterInfoValue.Trim().Length<startI+endI)return false;string mt=MeterInfoValue.Substring(startI,endI).ToUpperInvariant();var cp=MeterVariant.GetMeterTypeCode();int[] kv=cp.Where(x=>x.Key==mt).Select(p=>p.Value).ToArray();if(!kv.Contains(mode)){if(mt=="CC"&&mode==1)return true;return false;}return true;}catch{return false;}}
    public bool GetHTMeterTypes(){if(_set.MeterMode!=(int)MeterTypeInfo.Smart_Meter_3PH)if(MeterInfoValue.Contains("SM")||MeterInfoValue.Contains("FH"))return true;return false;}
    public int GetDisplayProgrammingVariant(){try{if(_set.MeterMode==(int)MeterTypeInfo.DLMS_3PH_RUBY)return(int)DisplayProgrammingTypes.OneByte;if(_set.MeterMode==(int)MeterTypeInfo.SAPPHIRE_S2)return(int)DisplayProgrammingTypes.TwoByte;return(int)DisplayProgrammingTypes.OneByte;}catch{return 0;}}
    public async Task<bool> ReadByteFromMeterAsync(byte[] obis,byte cls,byte att){int resp=_set.ApplicationContext==(byte)ApplicationContext.LogicalModeWithCiphering?await ReadDataCommandCypheredAsync(obis,cls,att):await ReadDataCommandAsync(obis,cls,att);GetWriteResponseCode=resp;return resp==(int)ProgrammingCode.Success;}
    public async Task<bool> ReadBlockFromMeterAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc){int r=_set.ApplicationContext==(byte)ApplicationContext.LogicalModeWithCiphering?await ReadDataBlockCommandCypheredAsync(obis,cls,att,acc,desc):await ReadDataBlockCommandAsync(obis,cls,att,acc,desc);return r==(int)ProgrammingCode.Success;}
    public async Task<int> ReadAllTamperBlockFromMeterAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc)=>_set.ApplicationContext==(byte)ApplicationContext.LogicalModeWithCiphering?await ReadDataBlockCommandCypheredAsync(obis,cls,att,acc,desc):await ReadDataBlockCommandAsync(obis,cls,att,acc,desc);
    public async Task<int> ReadDataCommandAsync(byte[] obis,byte cls,byte att)=>await Task.Run(()=>{try{BuildGetFrame(obis,cls,att);if(!_ser.fSendDataToPort(_cmd,_idx))return(int)ProgrammingCode.CosemConnectionFailed;_hdlc.fIncRecieve();if(!fCheckHDLCResponse(_ser.ReceiveBuffer))return(int)ProgrammingCode.CosemConnectionFailed;int r=_cosem.fCheckCOSEMResponseForGet(_ser.ReceiveBuffer);return r switch{0x01=>(int)ProgrammingCode.Success,0x0E=>(int)ProgrammingCode.DataUnavailable,0x02=>(int)ProgrammingCode.AccessDenied,0x03=>(int)ProgrammingCode.AccessDenied,_=>(int)ProgrammingCode.CosemConnectionFailed};}catch{return(int)ProgrammingCode.CosemConnectionFailed;}});
    public async Task<int> ReadDataCommandCypheredAsync(byte[] obis,byte cls,byte att)=>await Task.Run(()=>{try{BuildGetFrameCiphered(obis,cls,att);if(!_ser.fSendDataToPort(_cmd,_idx))return(int)ProgrammingCode.CosemConnectionFailed;int ivI=17;if(_ser.ReceiveBuffer[15]==0x81)ivI++;else if(_ser.ReceiveBuffer[15]==0x82)ivI+=2;byte[] gek=AesGcmCryptoService.HexToBytes(_set.GlobalEncryptionKey);byte[] ak=AesGcmCryptoService.HexToBytes(_set.AuthenticationKey);byte[] st=System.Text.Encoding.ASCII.GetBytes(_set.ClientSystemTitle);byte[] pt=_crypto.GetPlainTextFromCiphered(_ser.ReceiveBuffer,ivI,gek,ak,st);_hdlc.fIncRecieve();if(!fCheckHDLCResponse(_ser.ReceiveBuffer))return(int)ProgrammingCode.CosemConnectionFailed;if(pt.Length>0)Buffer.BlockCopy(pt,0,_ser.ReceiveBuffer,14,pt.Length);int r=_cosem.fCheckCOSEMResponseForGet(_ser.ReceiveBuffer);return r switch{0x01=>(int)ProgrammingCode.Success,0x0E=>(int)ProgrammingCode.DataUnavailable,0x02=>(int)ProgrammingCode.AccessDenied,_=>(int)ProgrammingCode.CosemConnectionFailed};}catch{return(int)ProgrammingCode.CosemConnectionFailed;}});
    public async Task<int> ReadDataBlockCommandAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc)=>await Task.Run(()=>{try{_cosem.nBlockIndex=0;_cosem.nBlockNumber=0;_idx=0;BuildGetFrame(obis,cls,att,acc,desc);if(!_ser.fSendDataToPort(_cmd,_idx))return(int)ProgrammingCode.CosemConnectionFailed;_hdlc.fIncRecieve();if(!fCheckHDLCResponse(_ser.ReceiveBuffer))return(int)ProgrammingCode.CosemConnectionFailed;int ret=_cosem.fCheckCOSEMResponse(_ser.ReceiveBuffer);if(ret==0x01)return(int)ProgrammingCode.Success;if(ret==0x05)return(int)ProgrammingCode.AccessDenied;if(ret==0x07)return(int)ProgrammingCode.DataUnavailable;if(ret!=0x02)return(int)ProgrammingCode.CosemConnectionFailed;while(true){_idx=0;BuildBlockNextFrame();_hdlc.fIncRecieve();if(!_ser.fSendDataToPort(_cmd,_idx))return(int)ProgrammingCode.CosemConnectionFailed;if(!fCheckHDLCResponse(_ser.ReceiveBuffer))return(int)ProgrammingCode.CosemConnectionFailed;ret=_cosem.fCheckCOSEMResponse(_ser.ReceiveBuffer);if(ret==0x01)break;if(ret==0x02)continue;return(int)ProgrammingCode.CosemConnectionFailed;}return(int)ProgrammingCode.Success;}catch{return(int)ProgrammingCode.CosemConnectionFailed;}});
    public async Task<int> ReadDataBlockCommandCypheredAsync(byte[] obis,byte cls,byte att,byte acc,List<byte> desc)=>await ReadDataBlockCommandAsync(obis,cls,att,acc,desc);
    public async Task<bool> WriteDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,List<byte> data,byte[] rt){int r=await Task.Run(()=>WriteParam(att,obis,cls,dt,dl,data,rt));GetWriteResponseCode=r;return r==(int)ProgrammingCode.Success;}
    int WriteParam(byte att,byte[] obis,byte cls,byte dt,byte dl,List<byte> data,byte[] rt){try{_idx=0;_idx=_hdlc.fAdd7E(_cmd,_idx);_idx=_hdlc.fAddHDLCFrameTag(_cmd,_idx);_idx=_hdlc.fAddServerSAP(_cmd,_idx,_set.ServerSAP,_set.ServerLowerMacAddress);_idx=_hdlc.fAddClientSAP(_cmd,_idx,_set.ClientSAP);_hdlc.fIncSend();_idx=_hdlc.fAddCmdByte(_cmd,_idx);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_idx=_cosem.fAddLLCByte(_cmd,_idx);_idx=_cosem.GetQueryWriteToMeter(data,_cmd,_idx,att,obis,cls,dt,dl,rt);_idx=_hdlc.FillWriteParameters(_cmd,_idx,data);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_hdlc.ffillLength(_cmd,_idx);_hdlc.fGenerateFCS(_cmd,1,8);_hdlc.fFillFCS(_cmd,9,10);_hdlc.fGenerateFCS(_cmd,1,_idx-3);_hdlc.fFillFCS(_cmd,_idx-2,_idx-1);_idx=_hdlc.fAdd7E(_cmd,_idx);if(!_ser.fSendDataToPort(_cmd,_idx))return(int)ProgrammingCode.CosemConnectionFailed;_hdlc.fIncRecieve();if(!fCheckHDLCResponse(_ser.ReceiveBuffer))return(int)ProgrammingCode.CosemConnectionFailed;int r=_cosem.fCheckCOSEMResponseForSet(_ser.ReceiveBuffer);return r==0x01?(int)ProgrammingCode.Success:r==0x02||r==0x04?(int)ProgrammingCode.AccessDenied:(int)ProgrammingCode.CosemConnectionFailed;}catch{return(int)ProgrammingCode.CosemConnectionFailed;}}
    public async Task<bool> WriteMethodToMeterAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,List<byte> data,byte[] rt,byte acc)=>await WriteDataToMeterAsync(att,obis,cls,dt,dl,data,rt);
    public async Task<bool> WriteBlockDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt)=>await WriteDataToMeterAsync(att,obis,cls,dt,(byte)dl,data,rt);
    public async Task<bool> WriteImageBlockDataToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt,List<byte> crc,List<byte> footer)=>await WriteDataToMeterAsync(att,obis,cls,dt,(byte)dl,data,rt);
    public async Task<bool> WriteImageActionNormalToMeterAsync(byte att,byte[] obis,byte cls,byte dt,int dl,List<byte> data,byte[] rt,List<byte> crc,List<byte> footer)=>await WriteDataToMeterAsync(att,obis,cls,dt,(byte)dl,data,rt);
    public async Task<bool> ReadBlockFromMeterBytesAsync(byte[] obis,byte cls,byte att,byte acc,byte[] desc)=>await ReadBlockFromMeterAsync(obis,cls,att,acc,desc.ToList());
    public async Task<bool> WriteDataToMeterBytesAsync(byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] data,byte[] rt)=>await WriteDataToMeterAsync(att,obis,cls,dt,dl,data.ToList(),rt);
    public async Task<bool> WriteBlockDataToMeterBytesAsync(byte att,byte[] obis,byte cls,byte dt,int dl,byte[] data,byte[] rt)=>await WriteBlockDataToMeterAsync(att,obis,cls,dt,dl,data.ToList(),rt);
    public async Task<int> ReadDataBlockCommandCypheredBytesAsync(byte[] obis,byte cls,byte att,byte acc,byte[] desc)=>await ReadDataBlockCommandCypheredAsync(obis,cls,att,acc,desc.ToList());
    public string[]? DLMSDataFormatorLabView(byte[] d,int idx,bool ascii){try{return DlmsHelper.DLMSDataFormator(d,idx,ascii);}catch{return null;}}
    public async Task<byte[]?> Read3PHDLMSCalibCoeffAsync(byte[] cmd)=>await Task.Run(()=>{try{if(!_ser.fSendDataToPort(cmd,cmd.Length))return null;if(_ser.BufferIndex<_ser.NoOfBytesToBeReceive3PHDLMSCalibCoeff)return null;byte[] r=new byte[_ser.BufferIndex+1];for(int i=0;i<_ser.BufferIndex;i++)r[i]=_ser.ReceiveBuffer[i];return r;}catch{return null;}});
    public List<byte> GetByteByEntryValueType(long f,long t)=>DlmsHelper.GetByteByEntryValueType(f,t);
    public List<byte> GetByteByEntryDateType(DateTime f,DateTime t)=>DlmsHelper.GetByteByEntryDateType(f,t);
    public List<string> GetMeterTypeList()=>MeterVariant.VisibleVariants.Select(v=>v.Name).ToList();
    public string GetSelectedMeterType(){var l=MeterVariant.AllVariants;int m=_set.GetMeterMode();return m>=0&&m<l.Count?l[m].Name:string.Empty;}
    public int DedicatedCommand(byte[] buf,int ni,string type,int len){if(!string.IsNullOrEmpty(_cosem.DedKeystr)&&type=="Read")buf[ni++]=0xD0;else if(string.IsNullOrEmpty(_cosem.DedKeystr)&&type=="Read")buf[ni++]=0xC8;else if(!string.IsNullOrEmpty(_cosem.DedKeystr)&&type=="Write")buf[ni++]=0xD1;else buf[ni++]=0xC9;if(len<128)buf[ni++]=0x00;else if(len<256){buf[ni++]=0x00;buf[ni++]=0x00;}else{buf[ni++]=0x00;buf[ni++]=0x00;buf[ni++]=0x00;}buf[ni]=0x00;byte sm=_set.GetSecurityMachanism();if(sm==0x01){_hdlc.SecuritysuitByte=0x20;buf[ni++]=0x20;}else buf[ni++]=(byte)_hdlc.SecuritysuitByte;buf[ni++]=(byte)((_hdlc.InitializationCounter>>24)&0xFF);buf[ni++]=(byte)((_hdlc.InitializationCounter>>16)&0xFF);buf[ni++]=(byte)((_hdlc.InitializationCounter>>8)&0xFF);buf[ni++]=(byte)(_hdlc.InitializationCounter&0xFF);return ni;}
    public bool fCheckHDLCResponse(byte[] b){try{return _hdlc.fCheckStartEndTag(b)&&_hdlc.fCheckFCS(b)&&_hdlc.fCheckServerSAP(b,_set.ClientSAP)&&_hdlc.fCheckCommand(b,_hdlc.nCMDByte);}catch{return false;}}
    void BuildGetFrame(byte[] obis,byte cls,byte att,byte acc=0,List<byte>? desc=null){_idx=0;_idx=_hdlc.fAdd7E(_cmd,_idx);_idx=_hdlc.fAddHDLCFrameTag(_cmd,_idx);_idx=_hdlc.fAddServerSAP(_cmd,_idx,_set.ServerSAP,_set.ServerLowerMacAddress);_idx=_hdlc.fAddClientSAP(_cmd,_idx,_set.ClientSAP);_hdlc.fIncSend();_idx=_hdlc.fAddCmdByte(_cmd,_idx);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_idx=_cosem.fAddLLCByte(_cmd,_idx);_idx=_cosem.GetQueryReadByClassOBIS(_cmd,_idx,att,obis,cls);if(acc!=0&&desc!=null)_idx=_cosem.FillCommandData(_cmd,--_idx,desc);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_hdlc.ffillLength(_cmd,_idx);_hdlc.fGenerateFCS(_cmd,1,8);_hdlc.fFillFCS(_cmd,9,10);_hdlc.fGenerateFCS(_cmd,1,_idx-3);_hdlc.fFillFCS(_cmd,_idx-2,_idx-1);_idx=_hdlc.fAdd7E(_cmd,_idx);}
    void BuildGetFrameCiphered(byte[] obis,byte cls,byte att){_idx=0;_idx=_hdlc.fAdd7E(_cmd,_idx);_idx=_hdlc.fAddHDLCFrameTag(_cmd,_idx);_idx=_hdlc.fAddServerSAP(_cmd,_idx,_set.ServerSAP,_set.ServerLowerMacAddress);_idx=_hdlc.fAddClientSAP(_cmd,_idx,_set.ClientSAP);_hdlc.fIncSend();_idx=_hdlc.fAddCmdByte(_cmd,_idx);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_idx=_cosem.fAddLLCByte(_cmd,_idx);_idx=DedicatedCommand(_cmd,_idx,"Read",0);int ci=_idx;_idx=_cosem.GetQueryReadByClassOBIS(_cmd,_idx,att,obis,cls);byte[] pt=new byte[_idx-ci];Buffer.BlockCopy(_cmd,ci,pt,0,pt.Length);_idx=ci;byte[] gek=AesGcmCryptoService.HexToBytes(_set.GlobalEncryptionKey);byte[] ak=AesGcmCryptoService.HexToBytes(_set.AuthenticationKey);byte[] st=System.Text.Encoding.ASCII.GetBytes(_set.ClientSystemTitle);byte[] enc=_crypto.CreateCipherCommand(pt,gek,ak,st,_hdlc.InitializationCounter,(byte)_set.Securitysuit);foreach(var b in enc)_cmd[_idx++]=b;_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_hdlc.ffillLength(_cmd,_idx);_cmd[15]=(byte)(_idx-18);_hdlc.fGenerateFCS(_cmd,1,8);_hdlc.fFillFCS(_cmd,9,10);_hdlc.fGenerateFCS(_cmd,1,_idx-3);_hdlc.fFillFCS(_cmd,_idx-2,_idx-1);_idx=_hdlc.fAdd7E(_cmd,_idx);}
    void BuildBlockNextFrame(){_idx=0;_idx=_hdlc.fAdd7E(_cmd,_idx);_idx=_hdlc.fAddHDLCFrameTag(_cmd,_idx);_idx=_hdlc.fAddServerSAP(_cmd,_idx,_set.ServerSAP,_set.ServerLowerMacAddress);_idx=_hdlc.fAddClientSAP(_cmd,_idx,_set.ClientSAP);_hdlc.fIncSend();_idx=_hdlc.fAddCmdByte(_cmd,_idx);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_idx=_cosem.fAddLLCByte(_cmd,_idx);_idx=_cosem.fGetBlockTransferPacket(_cmd,_idx);_idx=_hdlc.fAddBlankFCS(_cmd,_idx);_hdlc.ffillLength(_cmd,_idx);_hdlc.fGenerateFCS(_cmd,1,8);_hdlc.fFillFCS(_cmd,9,10);_hdlc.fGenerateFCS(_cmd,1,_idx-3);_hdlc.fFillFCS(_cmd,_idx-2,_idx-1);_idx=_hdlc.fAdd7E(_cmd,_idx);}
}
