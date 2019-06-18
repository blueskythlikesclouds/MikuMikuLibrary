using MikuMikuLibrary.Textures;
using System;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Controls
{
    public partial class TextureViewControl : UserControl
    {
        private static TextureViewControl sInstance;

        public static TextureViewControl Instance => sInstance ?? ( sInstance = new TextureViewControl() );

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        private Texture mTexture;
        private Bitmap[ , ] mBitmaps;
        private Bitmap mYcbcrBitmap;
        private int mMipMapIndex;
        private int mLevelIndex;

        private Color mOriginalForeColor;
        private Color mOriginalBackColor;

        public int CurrentMipMapIndex
        {
            get => mMipMapIndex;
            set
            {
                if ( mTexture.IsYCbCr )
                    return;

                value = Math.Max( 0, Math.Min( value, mTexture.MipMapCount - 1 ) );

                if ( value == mMipMapIndex )
                    return;

                mMipMapIndex = value;
                SetAll();
            }
        }

        public int CurrentLevelIndex
        {
            get => mLevelIndex;
            set
            {
                if ( mTexture.IsYCbCr )
                    return;

                value = Math.Max( 0, Math.Min( value, mTexture.Depth - 1 ) );

                if ( value == mLevelIndex )
                    return;

                mLevelIndex = value;
                SetAll();
            }
        }

        private void SetFormatText() =>
            mFormatLabel.Text = mTexture.IsYCbCr
                ? "Format: YCbCr"
                : $"Format: {Enum.GetName( typeof( TextureFormat ), mTexture.Format )}";

        private void SetSizeText() =>
            mSizeLabel.Text = mTexture.IsYCbCr
                ? $"Size: {mTexture.Width}x{mTexture.Height}"
                : $"Size: {mTexture[ mLevelIndex, mMipMapIndex ].Width}x{mTexture[ mLevelIndex, mMipMapIndex ].Height}";

        private void SetMipMapText() =>
            mMipMapLabel.Text = mTexture.IsYCbCr ? "MipMap: 1/1" : $"MipMap: {mMipMapIndex + 1}/{mTexture.MipMapCount}";

        private void SetLevelText() => mLevelLabel.Text =
            mTexture.IsYCbCr ? "Level: 1/1" : $"Level: {mLevelIndex + 1}/{mTexture.Depth}";

        private void SetControlBackground()
        {
            BackgroundImage = mTexture.IsYCbCr ? mYcbcrBitmap : mBitmaps[ mLevelIndex, mMipMapIndex ];

            BackgroundImageLayout =
                ClientSize.Width < BackgroundImage.Width || ClientSize.Height < BackgroundImage.Height
                    ? ImageLayout.Zoom
                    : ImageLayout.Center;

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

            mTexture = texture;

            if ( texture.IsYCbCr )
                mYcbcrBitmap = TextureDecoder.Decode( texture );

            else
            {
                mBitmaps = new Bitmap[ texture.Depth, texture.MipMapCount ];
                for ( int i = 0; i < texture.Depth; i++ )
                for ( int j = 0; j < texture.MipMapCount; j++ )
                    mBitmaps[ i, j ] = TextureDecoder.Decode( texture[ i, j ] );
            }

            SetAll();
        }

        private void DisposeBitmaps()
        {
            if ( mBitmaps != null )
            {
                foreach ( var bitmap in mBitmaps )
                    bitmap.Dispose();
            }

            mYcbcrBitmap?.Dispose();
        }

        protected override void OnLoad( EventArgs eventArgs )
        {
            mOriginalForeColor = mStatusStrip.ForeColor;
            mOriginalBackColor = mStatusStrip.BackColor;

            if ( StyleSet.CurrentStyle != null )
                ApplyStyle( StyleSet.CurrentStyle );

            StyleSet.StyleChanged += OnStyleChanged;

            base.OnLoad( eventArgs );
        }

        private void OnStyleChanged( object sender, StyleChangedEventArgs eventArgs )
        {
            ApplyStyle( eventArgs.Style );
        }

        private void ApplyStyle( Style style )
        {
            mStatusStrip.BackColor = style?.Background ?? mOriginalBackColor;
            mStatusStrip.ForeColor = style?.Foreground ?? mOriginalForeColor;

            Refresh();
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
                mComponents?.Dispose();
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
