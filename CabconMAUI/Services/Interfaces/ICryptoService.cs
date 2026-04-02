namespace CabconMAUI.Services.Interfaces;
public interface ICryptoService
{
    byte[] CreateCipherCommand(byte[] plaintext,byte[] globalEncKey,byte[] authKey,byte[] sysTitle,long ic,byte secSuite);
    byte[] GetPlainTextFromCiphered(byte[] buf,int ivIdx,byte[] globalEncKey,byte[] authKey,byte[] sysTitle);
}
