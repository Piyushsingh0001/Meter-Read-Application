using System.Security.Cryptography;
using CabconMAUI.Services.Interfaces;
namespace CabconMAUI.Services;
public class AesGcmCryptoService : ICryptoService
{
    public byte[] CreateCipherCommand(byte[] pt,byte[] gek,byte[] ak,byte[] sysTitle,long ic,byte ss)
    {
        byte[] iv=new byte[12];
        int tl=Math.Min(8,sysTitle.Length);Buffer.BlockCopy(sysTitle,0,iv,0,tl);
        iv[8]=(byte)((ic>>24)&0xFF);iv[9]=(byte)((ic>>16)&0xFF);iv[10]=(byte)((ic>>8)&0xFF);iv[11]=(byte)(ic&0xFF);
        byte[] aad=new byte[1+ak.Length];aad[0]=ss;Buffer.BlockCopy(ak,0,aad,1,ak.Length);
        byte[] ct=new byte[pt.Length];byte[] tag=new byte[12];
        using var aes=new AesGcm(gek,12);aes.Encrypt(iv,pt,ct,tag,aad);
        var r=new byte[1+1+4+ct.Length+tag.Length];int p=0;
        r[p++]=0xDB;r[p++]=ss;r[p++]=iv[8];r[p++]=iv[9];r[p++]=iv[10];r[p++]=iv[11];
        Buffer.BlockCopy(ct,0,r,p,ct.Length);p+=ct.Length;Buffer.BlockCopy(tag,0,r,p,tag.Length);
        return r;
    }
    public byte[] GetPlainTextFromCiphered(byte[] buf,int ivIdx,byte[] gek,byte[] ak,byte[] sysTitle)
    {
        try{
            byte ss=buf[ivIdx];byte[] ic=new byte[4];Buffer.BlockCopy(buf,ivIdx+1,ic,0,4);
            byte[] iv=new byte[12];int tl=Math.Min(8,sysTitle.Length);Buffer.BlockCopy(sysTitle,0,iv,0,tl);Buffer.BlockCopy(ic,0,iv,8,4);
            int ps=ivIdx+5;int pl=buf.Length-ps-12-1;if(pl<=0)return Array.Empty<byte>();
            byte[] ct=new byte[pl];byte[] tag=new byte[12];
            Buffer.BlockCopy(buf,ps,ct,0,pl);Buffer.BlockCopy(buf,ps+pl,tag,0,12);
            byte[] aad=new byte[1+ak.Length];aad[0]=ss;Buffer.BlockCopy(ak,0,aad,1,ak.Length);
            byte[] pt=new byte[pl];using var aes=new AesGcm(gek,12);aes.Decrypt(iv,ct,tag,pt,aad);return pt;
        }catch(Exception ex){System.Diagnostics.Debug.WriteLine($"[AES] {ex.Message}");return Array.Empty<byte>();}
    }
    public static byte[] HexToBytes(string hex){hex=hex.Replace(" ","").Replace("-","");var b=new byte[hex.Length/2];for(int i=0;i<b.Length;i++)b[i]=Convert.ToByte(hex.Substring(i*2,2),16);return b;}
}
