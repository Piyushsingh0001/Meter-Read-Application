namespace CabconMAUI.Services.Interfaces;
public interface IHdlcFrameService
{
    long InitializationCounter{get;set;} int SecuritysuitByte{get;set;} byte nCMDByte{get;}
    int  fAdd7E(byte[] buf,int idx); int fAddHDLCFrameTag(byte[] buf,int idx);
    int  fAddServerSAP(byte[] buf,int idx,int srv,int mac); int fAddClientSAP(byte[] buf,int idx,int cli);
    void fIncSend(); int fAddCmdByte(byte[] buf,int idx); int fAddBlankFCS(byte[] buf,int idx);
    void ffillLength(byte[] buf,int idx);
    void fGenerateFCS(byte[] buf,int start,int end); void fFillFCS(byte[] buf,int p1,int p2);
    int  FillWriteParameters(byte[] buf,int idx,List<byte> data);
    bool fCheckStartEndTag(byte[] buf); bool fCheckFCS(byte[] buf);
    bool fCheckServerSAP(byte[] buf,int cli); bool fCheckCommand(byte[] buf,byte exp);
    void fIncRecieve();
    bool fSendSNRM(int srv,int mac,int cli,int cbuf,int cbufR,int win,int winR);
    bool fSendDISC(int srv,int mac,int cli);
    bool IrDACheckBCC(byte[] buf); bool IrDACheckSyncWord(byte[] buf);
    bool IrDACheckCommandID(byte[] buf,byte cmd);
    bool IrDACheckBCC_1P(byte[] buf,int len); bool IrDACheckSyncWord_1P(byte[] buf,int len);
}
