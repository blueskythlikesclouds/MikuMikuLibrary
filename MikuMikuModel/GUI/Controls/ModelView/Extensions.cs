namespace MikuMikuModel.GUI.Controls.ModelView;

public static class Extensions
{
    public static Color ToColor(this Vector4 value) =>
        Color.FromArgb((int)(value.W * 255.0f), (int)(value.X * 255.0f), (int)(value.Y * 255.0f), (int)(value.Z * 255.0f));
}