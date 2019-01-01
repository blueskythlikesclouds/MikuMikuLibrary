using MikuMikuLibrary.Textures;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class TextureViewControl : UserControl
    {
        private static TextureViewControl instance;

        public static TextureViewControl Instance
        {
            get
            {
                if ( instance == null )
                    instance = new TextureViewControl();

                return instance;
            }
        }

        public static void DisposeInstance()
        {
            instance?.Dispose();
        }

        private Texture texture;
        private Bitmap[,] bitmaps;
        private Bitmap ycbcrBitmap;
        private int mipMapIndex;
        private int levelIndex;

        public int CurrentMipMapIndex
        {
            get => mipMapIndex;
            set
            {
                if ( texture.IsYCbCr )
                    return;

                value = Math.Max( 0, Math.Min( value, texture.MipMapCount - 1 ) );

                if ( value == mipMapIndex )
                    return;

                mipMapIndex = value;
                SetAll();
            }
        }

        public int CurrentLevelIndex
        {
            get => levelIndex;
            set
            {
                if ( texture.IsYCbCr )
                    return;

                value = Math.Max( 0, Math.Min( value, texture.Depth - 1 ) );

                if ( value == levelIndex )
                    return;

                levelIndex = value;
                SetAll();
            }
        }

        private void SetFormatText()
        {
            if ( texture.IsYCbCr )
                formatLabel.Text = "Format: YCbCr";
            else
                formatLabel.Text = $"Format: {Enum.GetName( typeof( TextureFormat ), texture.Format )}";
        }

        private void SetSizeText()
        {
            if ( texture.IsYCbCr )
                sizeLabel.Text = $"Size: {texture.Width}x{texture.Height}";
            else
                sizeLabel.Text = $"Size: {texture[ levelIndex, mipMapIndex ].Width}x{texture[ levelIndex, mipMapIndex ].Height}";
        }

        private void SetMipMapText()
        {
            if ( texture.IsYCbCr )
                mipMapLabel.Text = "MipMap: 1/1";
            else
                mipMapLabel.Text = $"MipMap: {mipMapIndex + 1}/{texture.MipMapCount}";
        }

        private void SetLevelText()
        {
            if ( texture.IsYCbCr )
                levelLabel.Text = "Level: 1/1";
            else
                levelLabel.Text = $"Level: {levelIndex + 1}/{texture.Depth}";
        }

        private void SetControlBackground()
        {
            if ( texture.IsYCbCr )
                BackgroundImage = ycbcrBitmap;
            else
                BackgroundImage = bitmaps[ levelIndex, mipMapIndex ];

            if ( ClientSize.Width < BackgroundImage.Width || ClientSize.Height < BackgroundImage.Height )
                BackgroundImageLayout = ImageLayout.Zoom;
            else
                BackgroundImageLayout = ImageLayout.Center;

            Refresh();
        }

        private void SetAll()
        {
            SetFormatText();
            SetSizeText();
            SetMipMapText();
            SetLevelText();
            SetControlBackground();
        }

        public void SetTexture( Texture texture )
        {
            DisposeBitmaps();

            this.texture = texture;

            if ( texture.IsYCbCr )
                ycbcrBitmap = TextureDecoder.Decode( texture );

            else
            {
                bitmaps = new Bitmap[ texture.Depth, texture.MipMapCount ];
                for ( int i = 0; i < texture.Depth; i++ )
                    for ( int j = 0; j < texture.MipMapCount; j++ )
                        bitmaps[ i, j ] = TextureDecoder.Decode( texture[ i, j ] );
            }

            SetAll();
        }

        private void DisposeBitmaps()
        {
            if ( bitmaps != null )
            {
                foreach ( var bitmap in bitmaps )
                    bitmap.Dispose();
            }

            ycbcrBitmap?.Dispose();
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            switch ( keyData )
            {
                case Keys.Up:
                    CurrentLevelIndex++;
                    return true;

                case Keys.Down:
                    CurrentLevelIndex--;
                    return true;

                case Keys.Left:
                    CurrentMipMapIndex--;
                    return true;

                case Keys.Right:
                    CurrentMipMapIndex++;
                    return true;
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                components?.Dispose();
                DisposeBitmaps();
            }
            base.Dispose( disposing );
        }

        public TextureViewControl()
        {
            InitializeComponent();
        }
    }
}
