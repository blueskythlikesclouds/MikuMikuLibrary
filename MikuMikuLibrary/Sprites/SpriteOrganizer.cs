using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Sprites
{
    internal class SpriteTree
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Sprite Sprite;
        public SpriteTree Left;
        public SpriteTree Right;

        public SpriteTree InsertSprite( Sprite sprite )
        {
            if ( Left != null || Right != null )
            {
                var node = Left.InsertSprite( sprite );
                if ( node != null )
                    return node;

                return Right.InsertSprite( sprite );
            }

            int spriteWidth = ( int ) sprite.Width + 1;
            int spriteHeight = ( int ) sprite.Height + 1;

            if ( Sprite != null )
                return null;

            if ( spriteWidth > Width || spriteHeight > Height )
                return null;

            if ( spriteWidth == Width && spriteHeight == Height )
            {
                Sprite = sprite;
                return this;
            }

            Left = new SpriteTree();
            Right = new SpriteTree();

            int w = Width - spriteWidth;
            int h = Height - spriteHeight;

            if ( w > h )
            {
                Left.X = X;
                Left.Y = Y;
                Left.Width = spriteWidth;
                Left.Height = Height;

                Right.X = X + spriteWidth;
                Right.Y = Y;
                Right.Width = Width - spriteWidth;
                Right.Height = Height;
            }

            else
            {
                Left.X = X;
                Left.Y = Y;
                Left.Width = Width;
                Left.Height = spriteHeight;

                Right.X = X;
                Right.Y = Y + spriteHeight;
                Right.Width = Width;
                Right.Height = Height - spriteHeight;
            }

            return Left.InsertSprite( sprite );
        }

        public void UpdateSprites()
        {
            if ( Sprite != null )
            {
                Sprite.X = X;
                Sprite.Y = Y;
            }

            Left?.UpdateSprites();
            Right?.UpdateSprites();
        }
    }

    public static class SpriteOrganizer
    {
        private static PackSpritesReturnData OrganizeSprites( int width, int height, int maxSize, List<Sprite> sprites )
        {
            var perfectFit = sprites.FirstOrDefault(
                x => x.Width == maxSize && x.Height == maxSize );

            if ( perfectFit != null )
            {
                perfectFit.X = 0;
                perfectFit.Y = 0;

                var unFitSprites = new List<Sprite>( sprites );
                unFitSprites.Remove( perfectFit );

                return new PackSpritesReturnData
                {
                    OrganizedSprites = new List<Sprite> { perfectFit },
                    UnorganizedSprites = unFitSprites,
                    Width = maxSize,
                    Height = maxSize
                };
            }

            bool allOrganized = false;
            while ( !allOrganized )
            {
                var spriteTree = new SpriteTree
                {
                    Width = width,
                    Height = height
                };

                allOrganized = true;

                var unFitSprites = new List<Sprite>();
                foreach ( var sprite in sprites )
                    if ( spriteTree.InsertSprite( sprite ) == null )
                    {
                        allOrganized = false;
                        unFitSprites.Add( sprite );
                    }

                if ( allOrganized )
                {
                    spriteTree.UpdateSprites();
                }
                else
                {
                    if ( width < maxSize || height < maxSize )
                    {
                        if ( height < width )
                            height *= 2;
                        else
                            width *= 2;
                    }

                    else
                    {
                        spriteTree.UpdateSprites();

                        return new PackSpritesReturnData
                        {
                            OrganizedSprites = sprites.Except( unFitSprites ).ToList(),
                            UnorganizedSprites = unFitSprites,
                            Width = width,
                            Height = height
                        };
                    }
                }
            }

            return new PackSpritesReturnData
            {
                OrganizedSprites = sprites,
                UnorganizedSprites = new List<Sprite>(),
                Width = width,
                Height = height
            };
        }

        private static IEnumerable<PackSpritesReturnData> OrganizeSprites( List<Sprite> sprites, int maxSize )
        {
            maxSize = Math.Max( maxSize,
                ( int ) Math.Max( sprites.Max( x => x.Width ), sprites.Max( x => x.Height ) ) );

            var spritesToOrganize = new List<Sprite>( sprites );
            while ( spritesToOrganize.Count != 0 )
            {
                var spritesToOrganizeForThisTurn = new List<Sprite>();
                int filledPixelCount = 0;
                int evaluatedPixelCount = 0;
                var evaluatedSprites = new List<Sprite>();
                var failedSprites = new List<Sprite>();
                int textureWidth = 4;
                int textureHeight = 4;
                int texturePixelCount = textureWidth * textureHeight;
                int nextTextureWidth = textureWidth * 2;
                int nextTextureHeight = textureHeight;
                int nextTexturePixelCount = nextTextureWidth * nextTextureHeight;
                int nextExtraPixelCount = nextTexturePixelCount - texturePixelCount;

                var sortedSprites = spritesToOrganize.OrderByDescending( x => x.Width * x.Height ).ToList();
                foreach ( var sprite in sortedSprites )
                {
                    int spriteWidth = ( int ) sprite.Width;
                    int spriteHeight = ( int ) sprite.Height;
                    int spritePixelCount = spriteWidth * spriteHeight;

                    bool adding = true;
                    while ( adding )
                        if ( filledPixelCount + evaluatedPixelCount + spritePixelCount <= nextTexturePixelCount )
                        {
                            evaluatedPixelCount += spritePixelCount;
                            evaluatedSprites.Add( sprite );
                            adding = false;
                        }
                        else
                        {
                            textureWidth = nextTextureWidth;
                            textureHeight = nextTextureHeight;
                            texturePixelCount = nextTexturePixelCount;

                            if ( textureWidth < maxSize || textureHeight < maxSize )
                            {
                                if ( nextTextureHeight < nextTextureWidth )
                                    nextTextureHeight = nextTextureHeight * 2;
                                else
                                    nextTextureWidth = nextTextureWidth * 2;

                                nextTexturePixelCount = nextTextureWidth * nextTextureHeight;
                                nextExtraPixelCount = nextTexturePixelCount - texturePixelCount;

                                spritesToOrganizeForThisTurn.AddRange( evaluatedSprites );

                                filledPixelCount += evaluatedPixelCount;
                                evaluatedPixelCount = 0;
                                evaluatedSprites.Clear();
                            }

                            else
                            {
                                break;
                            }
                        }

                    if ( adding ) failedSprites.Add( sprite );
                }

                spritesToOrganize.Clear();
                spritesToOrganize.AddRange( failedSprites );

                if ( evaluatedSprites.Count != 0 )
                {
                    bool successCondition = evaluatedPixelCount > nextExtraPixelCount / 2;
                    foreach ( var sprite in evaluatedSprites )
                        if ( successCondition )
                            spritesToOrganizeForThisTurn.Add( sprite );
                        else
                            spritesToOrganize.Add( sprite );

                    if ( successCondition )
                    {
                        textureWidth = nextTextureWidth;
                        textureHeight = nextTextureHeight;
                    }
                }

                var organizedSpriteData =
                    OrganizeSprites( textureWidth, textureHeight, maxSize, spritesToOrganizeForThisTurn );
                spritesToOrganize.AddRange( organizedSpriteData.UnorganizedSprites );

                yield return organizedSpriteData;
            }
        }

        private struct PackSpritesReturnData
        {
            public List<Sprite> OrganizedSprites;
            public List<Sprite> UnorganizedSprites;
            public int Width;
            public int Height;
        }

        //public static TextureSet Organize( Dictionary<Sprite, Bitmap> sprites )
        //{
        //    foreach ( var sprite in sprites )
        //    {
        //        sprite.Key.Width = sprite.Value.Width;
        //        sprite.Key.Height = sprite.Value.Height;
        //    }

        //    var textureSet = new TextureSet();

        //    foreach ( var data in OrganizeSprites( sprites.Keys.ToList(), 2048 ) )
        //    {
        //        using ( var bitmap = new Bitmap( data.Width, data.Height ) )
        //        {
        //            using ( var gfx = Graphics.FromImage( bitmap ) )
        //            {
        //                foreach ( var sprite in data.OrganizedSprites )
        //                {
        //                    gfx.DrawImageUnscaled( sprites[ sprite ], ( int )sprite.X, ( int )sprite.Y );
        //                    sprite.TextureIndex = textureSet.Textures.Count;
        //                }
        //            }

        //            bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );

        //            var texture = TextureEncoder.Encode( bitmap, TextureFormat.RGBA, false );
        //            texture.Name = string.Format( "MERGE_NOCOMP_{0:000}_0", textureSet.Textures.Count );

        //            textureSet.Textures.Add( texture );
        //        }
        //    }

        //    return textureSet;
        //}
    }
}