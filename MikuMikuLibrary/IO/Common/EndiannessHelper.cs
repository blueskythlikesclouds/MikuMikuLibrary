namespace MikuMikuLibrary.IO.Common;

public class EndiannessHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Reverse<T>(ref T value) where T : unmanaged
    {
        void* pV = Unsafe.AsPointer(ref value);
        int* pI = (int*)pV;

        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(Half))
            *(short*)pV = BinaryPrimitives.ReverseEndianness(*(short*)pV);

        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float))
            *(int*)pV = BinaryPrimitives.ReverseEndianness(*(int*)pV);

        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double))
            *(long*)pV = BinaryPrimitives.ReverseEndianness(*(long*)pV);

        else if (typeof(T) == typeof(Vector2))
        {
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
        }
        else if (typeof(T) == typeof(Vector4))
        {
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
        }
        else if (typeof(T) == typeof(Matrix4x4))
        {
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
            pI++;
            *pI = BinaryPrimitives.ReverseEndianness(*pI);
        }
    }
}