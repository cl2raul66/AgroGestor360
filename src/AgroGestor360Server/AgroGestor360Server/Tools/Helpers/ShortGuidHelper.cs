namespace AgroGestor360Server.Tools.Helpers;

public class ShortGuidHelper
{
    public static string Generate()
    {
        string guidString = Guid.NewGuid().ToString();

        string lastGroup = guidString.Substring(guidString!.Length - 12);

        return lastGroup.ToUpper();
    }
}
