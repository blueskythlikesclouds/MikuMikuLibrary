using System.Drawing;
using System.Windows.Forms;

namespace MikuMikuModel.GUI.Controls
{
    public partial class SpriteViewControl : UserControl
    {
        private static SpriteViewControl sInstance;

        private Bitmap mBitmap;

        public static SpriteViewControl Instance => sInstance ?? (sInstance = new SpriteViewControl());

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SpriteViewControl
            // 
            this.DoubleBuffered = true;
            this.Name = "SpriteViewControl";
            this.ResumeLayout(false);

        }

        public void SetBitmap(Bitmap bitmap)
        {
            if (mBitmap != null) mBitmap.Dispose();
            mBitmap = bitmap;
            BackgroundImage = mBitmap;

            if (mBitmap != null)
            {
                BackgroundImageLayout =
                ClientSize.Width < BackgroundImage.Width || ClientSize.Height < BackgroundImage.Height
                    ? ImageLayout.Zoom
                    : ImageLayout.Center;
            }

            Refresh();
        }
    }
}
