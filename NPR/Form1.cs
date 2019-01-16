using System;
using System.Drawing;
using System.Windows.Forms;

namespace NPR
{
    public partial class Form1 : Form
    {
        // The original image.
        private Bitmap OriginalImage;
        private Bitmap outBm;
        private Bitmap area;

        // True when we're selecting a rectangle.
        private bool IsSelecting = false;

        // The area we are selecting.
        private int X0, Y0, X1, Y1;

        public Form1()
        {
            InitializeComponent();
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OriginalImage = (Bitmap)Image.FromFile(openFileDialog.FileName);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Image = OriginalImage;
            }
        }

        private void floydSteinbergToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int r, r1, r2, r3, r4;
            int error;
            int s;
            if (area == null)
            {
                MessageBox.Show("No image loaded, please select File -> Load Image, then try again.", "No image loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (area.Width > area.Height)
                s = area.Height;
            else
                s = area.Width;
            outBm = new Bitmap(s, s);

            for (int i = s - 2; i > 0; i--)
                for (int j = 1; j < s - 1; j++)
                {
                    int c = area.GetPixel(i, j).R;
                    r = Approximate(c);
                    outBm.SetPixel(i, j, Color.FromArgb(r, r, r));
                    error = c - r;
                    r1 = area.GetPixel(i, j + 1).R + 7 * error / 16;
                    r2 = area.GetPixel(i + 1, j - 1).R + 3 * error / 16;
                    r3 = area.GetPixel(i + 1, j).R + 5 * error / 16;
                    r4 = area.GetPixel(i + 1, j + 1).R + error / 16;

                    if (r1 >= 255) r1 = 255;
                    if (r2 >= 255) r2 = 255;
                    if (r3 >= 255) r3 = 255;
                    if (r4 >= 255) r4 = 255;
                    outBm.SetPixel(i, j + 1, Color.FromArgb(r1, r1, r1));
                    outBm.SetPixel(i + 1, j - 1, Color.FromArgb(r2, r2, r2));
                    outBm.SetPixel(i + 1, j, Color.FromArgb(r3, r3, r3));
                    outBm.SetPixel(i + 1, j + 1, Color.FromArgb(r4, r4, r4));

                }
            pictureBox.Image = outBm;

        }
    

        private void picOriginal_MouseDown(object sender, MouseEventArgs e)
        {
            if (OriginalImage == null) return;
            IsSelecting = true;

            // Save the start point.
            X0 = e.X;
            Y0 = e.Y;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {   if (OriginalImage == null) return;
            // Do nothing it we're not selecting an area.
            if (!IsSelecting) return;
            IsSelecting = false;

            // Copy the selected part of the image.
            int wid = Math.Abs(X0 - X1);
            int hgt = Math.Abs(Y0 - Y1);
            if ((wid < 1) || (hgt < 1)) return;

            area = new Bitmap(wid, hgt);
            using (Graphics gr = Graphics.FromImage(area))
            {
                Rectangle source_rectangle =
                    new Rectangle(Math.Min(X0, X1), Math.Min(Y0, Y1),
                        wid, hgt);
                Rectangle dest_rectangle =
                    new Rectangle(0, 0, wid, hgt);
                gr.DrawImage(OriginalImage, dest_rectangle,
                    source_rectangle, GraphicsUnit.Pixel);
            }

        }

        private int Approximate(int c)
        {
            if (c >= 0 && c < 63)
                return 0;
            if (c >= 63 && c < 127)
                return 63;
            if (c >= 127 && c < 191)
                return 127;
            if (c >= 191 && c < 255)
                return 191;
            else
                return 0;
        }

        private void picOriginal_MouseMove(object sender, MouseEventArgs e)
        {
            if (OriginalImage == null) return;
            // Do nothing it we're not selecting an area.
            if (!IsSelecting) return;

            // Save the new point.
            X1 = e.X;
            Y1 = e.Y;

            // Make a Bitmap to display the selection rectangle.
            Bitmap bm = new Bitmap(OriginalImage);

            // Draw the rectangle.
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawRectangle(Pens.Red,
                    Math.Min(X0, X1), Math.Min(Y0, Y1),
                    Math.Abs(X0 - X1), Math.Abs(Y0 - Y1));
            }

            // Display the temporary bitmap.
            pictureBox.Image = bm;
        }
    }
}