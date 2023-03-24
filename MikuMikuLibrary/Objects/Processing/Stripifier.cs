using System.Collections.Generic;

namespace MikuMikuLibrary.Objects.Processing;

public static class Stripifier
{
    public static uint[] Stripify(uint[] indices)
    {
        return Native.Stripifier.Stripify(indices);
    }

    public static uint[] Unstripify(uint[] indices)
    {
        var triangleIndices = new List<uint>((indices.Length - 2) * 3);

        uint a = indices[0];
        uint b = indices[1];
        bool direction = false;

        for (int i = 2; i < indices.Length; i++)
        {
            uint c = indices[i];

            if (c == 0xFFFFFFFF)
            {
                a = indices[++i];
                b = indices[++i];
                direction = false;
            }
            else
            {
                direction = !direction;
                if (a != b && b != c && c != a)
                {
                    if (direction)
                    {
                        triangleIndices.Add(a);
                        triangleIndices.Add(b);
                        triangleIndices.Add(c);
                    }
                    else
                    {
                        triangleIndices.Add(a);
                        triangleIndices.Add(c);
                        triangleIndices.Add(b);
                    }
                }

                a = b;
                b = c;
            }
        }

        return triangleIndices.ToArray();
    }
}