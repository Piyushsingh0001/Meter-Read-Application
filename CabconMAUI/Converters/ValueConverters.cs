using System.Globalization;
namespace CabconMAUI.Converters;
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? v,Type t,object? p,CultureInfo c)=>v is string s&&!string.IsNullOrWhiteSpace(s);
    public object ConvertBack(object? v,Type t,object? p,CultureInfo c)=>throw new NotImplementedException();
}
public class InvertBoolConverter : IValueConverter
{
    public object Convert(object? v,Type t,object? p,CultureInfo c)=>v is bool b&&!b;
    public object ConvertBack(object? v,Type t,object? p,CultureInfo c)=>v is bool b&&!b;
}
public class BoolToErrorColorConverter : IValueConverter
{
    public object Convert(object? v,Type t,object? p,CultureInfo c)=>(v is bool b&&b)?Color.FromArgb("#C0392B"):Color.FromArgb("#1E8449");
    public object ConvertBack(object? v,Type t,object? p,CultureInfo c)=>throw new NotImplementedException();
}
public class BoolToErrorBgConverter : IValueConverter
{
    public object Convert(object? v,Type t,object? p,CultureInfo c)=>(v is bool b&&b)?Color.FromArgb("#FDEDEC"):Color.FromArgb("#EAFAF1");
    public object ConvertBack(object? v,Type t,object? p,CultureInfo c)=>throw new NotImplementedException();
}
