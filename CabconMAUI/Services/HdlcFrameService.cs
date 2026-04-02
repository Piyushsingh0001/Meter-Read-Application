using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class HdlcFrameService : IHdlcFrameService
{
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
    public void ffillLength(byte[] b,int idx){int l=idx-2;if(l<128)b[1]=(byte)l;else{b[1]=0x81;b[2]=(byte)l;}}
    public void fGenerateFCS(byte[] b,int s,int e){_lastFcs=FCS16(b,s,e-s);}
    public void fFillFCS(byte[] b,int p1,int p2){b[p1]=(byte)(_lastFcs&0xFF);b[p2]=(byte)((_lastFcs>>8)&0xFF);}
    public int FillWriteParameters(byte[] b,int i,List<byte> data){foreach(var x in data)b[i++]=x;return i;}
    public bool fCheckStartEndTag(byte[] b)=>b!=null&&b.Length>=4&&b[0]==0x7E&&b[b.Length-1]==0x7E;
    public bool fCheckFCS(byte[] b){try{int l=b.Length;ushort h=FCS16(b,1,8);if(b[9]!=(byte)(h&0xFF)||b[10]!=(byte)((h>>8)&0xFF))return false;ushort t=FCS16(b,1,l-4);return b[l-3]==(byte)(t&0xFF)&&b[l-2]==(byte)((t>>8)&0xFF);}catch{return false;}}
    public bool fCheckServerSAP(byte[] b,int cli){try{return b[7]==(byte)(((cli<<1)|0x01)&0xFF);}catch{return false;}}
    public bool fCheckCommand(byte[] b,byte e){try{return b[11]==(e|0x01);}catch{return false;}}
    public void fIncRecieve(){_rcv=(byte)((_rcv+1)&0x07);_cmd=(byte)((_snd<<1)|(_rcv<<5)|0x01);}
    public bool fSendSNRM(int srv,int mac,int cli,int cb,int cbR,int w,int wR){System.Diagnostics.Debug.WriteLine("[HDLC] fSendSNRM (stub)");return true;}
    public bool fSendDISC(int srv,int mac,int cli){System.Diagnostics.Debug.WriteLine("[HDLC] fSendDISC (stub)");return true;}
    public bool IrDACheckBCC(byte[] b){if(b==null||b.Length<2)return false;byte x=0;for(int i=1;i<b.Length-1;i++)x^=b[i];return x==b[b.Length-1];}
    public bool IrDACheckSyncWord(byte[] b)=>b!=null&&b.Length>=2&&b[0]==0x95&&b[1]==0x95;
    public bool IrDACheckCommandID(byte[] b,byte c)=>b!=null&&b.Length>=7&&b[6]==c;
    public bool IrDACheckBCC_1P(byte[] b,int l){if(b==null||l<1)return false;byte x=0;for(int i=0;i<l-1;i++)x^=b[i];return x==b[l-1];}
    public bool IrDACheckSyncWord_1P(byte[] b,int l)=>b!=null&&l>=2&&b[0]==0x68&&b[1]==0x68;
    public static ushort FCS16(byte[] d,int s,int n){ushort c=0xFFFF;for(int i=s;i<s+n;i++){c^=d[i];for(int j=0;j<8;j++)c=(ushort)((c&1)!=0?(c>>1)^0x8408:c>>1);}return(ushort)(c^0xFFFF);}
}
