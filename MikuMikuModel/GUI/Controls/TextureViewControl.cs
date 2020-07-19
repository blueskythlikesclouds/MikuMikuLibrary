using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;
using MikuMikuModel.Resources.Styles;

namespace MikuMikuModel.GUI.Controls
{
    public partial class TextureViewControl : UserControl
    {
        private static TextureViewControl sInstance;

        private Texture mTexture;
        private Bitmap[ , ] mBitmaps;
        private Bitmap mYcbcrBitmap;
        private int mMipMapIndex;
        private int mArrayIndex;

        public static TextureViewControl Instance => sInstance ?? ( sInstance = new TextureViewControl() );

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
            get => mArrayIndex;
            set
            {
                if ( mTexture.IsYCbCr )
                    return;

                value = Math.Max( 0, Math.Min( value, mTexture.ArraySize - 1 ) );

                if ( value == mArrayIndex )
                    return;

                mArrayIndex = value;
                SetAll();
            }
        }

        public static void ResetInstance()
        {
            sInstance?.DisposeBitmaps();
        }

        public static void DisposeInstance()
        {
            sInstance?.Dispose();
        }

        private void SetFormatText()
        {
            mFormatLabel.Text = mTexture.IsYCbCr
                ? "Format: YCbCr"
                : $"Format: {Enum.GetName( typeof( TextureFormat ), mTexture.Format )}";
        }

        private void SetSizeText()
        {
            mSizeLabel.Text = mTexture.IsYCbCr
                ? $"Size: {mTexture.Width}x{mTexture.Height}"
                : $"Size: {mTexture[ mArrayIndex, mMipMapIndex ].Width}x{mTexture[ mArrayIndex, mMipMapIndex ].Height}";
        }

        private void SetMipMapText()
        {
            mMipMapLabel.Text = mTexture.IsYCbCr ? "MipMap: 1/1" : $"MipMap: {mMipMapIndex + 1}/{mTexture.MipMapCount}";
        }

        private void SetLevelText()
        {
            mLevelLabel.Text = mTexture.IsYCbCr ? "Level: 1/1" : $"Level: {mArrayIndex + 1}/{mTexture.ArraySize}";
        }

        private void SetControlBackground()
        {
            BackgroundImage = mTexture.IsYCbCr ? mYcbcrBitmap : mBitmaps[ mArrayIndex, mMipMapIndex ];

            BackgroundImageLayout = ClientSize.Width < BackgroundImage.Width || ClientSize.Height < BackgroundImage.Height
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
            if ( mTexture == texture )
                return;

            DisposeBitmaps();

            mTexture = texture;

            if ( texture.IsYCbCr )
                mYcbcrBitmap = TextureDecoder.DecodeToBitmap( texture );

            else
                mBitmaps = TextureDecoder.DecodeToBitmaps( texture );

            mArrayIndex = 0;
            mMipMapIndex = 0;

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

            mTexture = null;
        }

        private void OnStyleChanged( object sender, StyleChangedEventArgs eventArgs )
        {
            StyleHelpers.ApplyStyle( this, eventArgs.Style );

            Refresh();
        }

        protected override void OnLoad( EventArgs eventArgs )
        {
            StyleHelpers.StoreDefaultStyle( this );

            if ( StyleSet.CurrentStyle != null )
                StyleHelpers.ApplyStyle( this, StyleSet.CurrentStyle );

            StyleSet.StyleChanged += OnStyleChanged;

            base.OnLoad( eventArgs );
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
        ///     Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                DisposeBitmaps();
                StyleSet.StyleChanged -= OnStyleChanged;
            }

            base.Dispose( disposing );
        }

        public TextureViewControl()
        {
            InitializeComponent();
        }
    }
}