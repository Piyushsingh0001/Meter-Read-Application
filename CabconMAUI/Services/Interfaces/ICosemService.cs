namespace CabconMAUI.Services.Interfaces;
public interface ICosemService
{
    int nBlockIndex{get;set;} int nBlockNumber{get;set;} int nTotalPacketSize{get;set;}
    int nMaxBufferSize{get;set;} string DedKeystr{get;set;}
    int  fAddLLCByte(byte[] buf,int idx);
    int  GetQueryReadByClassOBIS(byte[] buf,int idx,byte att,byte[] obis,byte cls);
    int  GetQueryWriteToMeter(List<byte> data,byte[] buf,int idx,byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] rt);
    int  GetQueryWriteMethodToMeter(byte[] buf,int idx,byte att,byte[] obis,byte cls,byte dt,byte dl,byte[] rt,byte acc);
    int  GetQueryToWriteBlockToMeter(byte[] buf,int idx,byte att,byte[] obis,byte cls,byte dt,int dl,byte[] rt);
    int  GetQueryToWriteBlockToMeterWithoutAccessSelector(byte[] buf,int idx,byte att,byte[] obis,byte cls,byte dt,int dl,byte[] rt);
    int  FillCommandData(byte[] buf,int idx,List<byte> desc);
    int  fGetBlockTransferPacket(byte[] buf,int idx);
    int  fSetBlockTransferPacket(byte[] buf,int idx,byte[] data,bool blk);
    int  fSetImgBlockTransferPacket(byte[] buf,int idx,byte[] data,bool blk,object? extra);
    int  fActionNormalImgBlockTransferPacket(byte[] buf,int idx,byte[] data,object? extra);
    int  fCheckCOSEMResponseForGet(byte[] buf); int fCheckCOSEMResponseForSet(byte[] buf);
    int  fCheckCOSEMResponse(byte[] buf); int fCheckCOSEMResponseForImageBlockSet(byte[] buf);
    bool fSendAARQ(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,byte ac);
    bool fSendAARQ_Cyphered(int srv,int mac,int cli,byte sm,string pwd,string hlsKey,string hlsPwd,string cb,int pdu,string cst,int ss,string gek,string ak,int dk);
}
