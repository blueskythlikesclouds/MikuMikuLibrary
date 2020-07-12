using System.Collections.Generic;
using System.Drawing;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;

namespace MikuMikuLibrary.Sprites
{
    public static class SpriteCropper
    {
        public static Bitmap Crop( Sprite sprite, SpriteSet parentSet )
        {
            if ( sprite.TextureIndex >= parentSet.TextureSet.Textures.Count )
                return null;

            var texture = parentSet.TextureSet.Textures[ ( int ) sprite.TextureIndex ];

            if ( sprite.Width <= 0 || sprite.Height <= 0 ||
                 sprite.X + sprite.Width > texture.Width || sprite.Y + sprite.Height > texture.Height )
                return null;

            var bitmap = TextureDecoder.DecodeToBitmap( texture );

            bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

            Bitmap croppedBitmap = bitmap.Clone(
                new RectangleF( sprite.X, sprite.Y, sprite.Width, sprite.Height ), bitmap.PixelFormat );

            bitmap.Dispose();
            return croppedBitmap;
        }

        public static Dictionary<Sprite, Bitmap> Crop( SpriteSet spriteSet )
        {
            var bitmaps = new List<Bitmap>( spriteSet.TextureSet.Textures.Count );
            foreach ( var texture in spriteSet.TextureSet.Textures )
            {
                var bitmap = TextureDecoder.DecodeToBitmap( texture );
                bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );
                bitmaps.Add( bitmap );
            }

            var sprites = new Dictionary<Sprite, Bitmap>( spriteSet.Sprites.Count );
            foreach ( var sprite in spriteSet.Sprites )
            {
                var sourceBitmap = bitmaps[ ( int ) sprite.TextureIndex ];
                var bitmap = sourceBitmap.Clone(
                    new RectangleF( sprite.X, sprite.Y, sprite.Width, sprite.Height ), sourceBitmap.PixelFormat );
                sprites.Add( sprite, bitmap );
            }

            foreach ( var bitmap in bitmaps )
                bitmap.Dispose();

            return sprites;
        }
    }
}