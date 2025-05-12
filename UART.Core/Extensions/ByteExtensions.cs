namespace UART.Core.Extensions;

public static class ByteExtensions
{
    /// <summary>
    /// With thanks to  @jjxtra on Github. The code has already been created and there's no need to reinvent the wheel is there?
    /// </summary>
    public static IEnumerable<int> PatternAt(this byte[] source, byte[] pattern)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                yield return i;
            }
        }
    }
}