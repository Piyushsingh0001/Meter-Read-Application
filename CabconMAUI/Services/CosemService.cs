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
}
