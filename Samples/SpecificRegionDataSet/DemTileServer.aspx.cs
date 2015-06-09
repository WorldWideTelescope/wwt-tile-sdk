using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Xml;
using System.Text;
using System.Net;

public partial class DemTileServer : System.Web.UI.Page
{
    // Customize this path to match the location of your pyramid
    const string pngFormat = @"<Path to collection>\Pyramid\{1}\{2}\L{1}X{2}Y{3}.png";
    const string demFormat = @"<Path to collection>\Pyramid\{1}\{2}\DL{1}X{2}Y{3}.bin";

    private static byte[] s_transparentTileData = null;
    private static object s_lock = new Object();

    protected void Page_Load(object sender, EventArgs e)
    {
        string projectionType = null;

        // Everything below should be unchanged.
        string query = Request.Params["Q"];
        if (!string.IsNullOrEmpty(query))
        {
            string[] values = query.Split(',');
            if ((values != null) && (values.Length == 5))
            {
                int level = Convert.ToInt32(values[0]);
                int x = Convert.ToInt32(values[1]);
                int y = Convert.ToInt32(values[2]);
                projectionType = values[3];
                string ext = values[4];

                if ("png" == ext.ToLowerInvariant())
                {
                    byte[] data = GetTile(projectionType, level, x, y, pngFormat);
                    if (data == null)
                        data = GetTransparentTile();

                    Response.ContentType = "image/png";
                    Response.OutputStream.Write(data, 0, data.Length);
                    Response.Flush();
                    Response.End();
                    return;
                }
                else if (ext.ToLowerInvariant().StartsWith("dem"))
                {
                    Response.ContentType = "application/octet-stream";
                    byte[] data = GetTile(projectionType, level, x, y, demFormat);
                    if (data == null)
                    {
                        int demSize;
                        if (!int.TryParse(ext.Substring(3), out demSize))
                            demSize = 0;
                        data = new byte[demSize];
                    }

                    Response.OutputStream.Write(data, 0, data.Length);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        if (string.IsNullOrEmpty(projectionType))
        {
            Help();
            return;
        }
    }

    private byte[] GetTile(string projectionType, int level, int x, int y, string format)
    {
        byte[] data = null;
        string filename = string.Format(format, projectionType, level, x, y);
        if (File.Exists(filename))
        {
            using (Stream s = File.OpenRead(filename))
            {
                int length = (int)s.Length;
                data = new byte[length];
                s.Read(data, 0, length);
            }
        }
        return data;
    }

    private static byte[] GetTransparentTile()
    {
        if (s_transparentTileData == null)
        {
            lock (s_lock)
            {
                using (Bitmap b = new Bitmap(256, 256))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.FromArgb(0, 0, 0, 0));
                    }
                    using (MemoryStream s = new MemoryStream())
                    {
                        b.Save(s, ImageFormat.Png);
                        s.Seek(0, SeekOrigin.Begin);
                        int length = (int)s.Length;
                        s_transparentTileData = new byte[length];
                        s.Read(s_transparentTileData, 0, length);
                    }
                }
            }
        }

        return s_transparentTileData;
    }

    private void Help()
    {
        byte[] data;
        using (MemoryStream s = new MemoryStream())
        using (Bitmap bmp = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Font font = new Font("Arial", 12);
                SolidBrush brush = new SolidBrush(Color.White);
                g.DrawString("Check request params!", font, brush, new PointF(10, 10));
            }

            bmp.Save(s, ImageFormat.Jpeg);
            s.Position = 0;
            int length = (int)s.Length;
            data = new byte[length];
            s.Read(data, 0, length);
        }

        Response.ContentType = "image/jpeg";
        Response.OutputStream.Write(data, 0, data.Length);
        Response.Flush();
        Response.End();
    }
}