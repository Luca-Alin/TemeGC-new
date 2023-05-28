using System.Windows.Forms;

namespace TemeGC
{
    internal static class S1
    {
        public static PictureBox P1(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            Graphics g = pb.CreateGraphics();
            
            return pb;
        }

        public static PictureBox P2(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);
            
            return pb;
        }

        public static PictureBox P3(PictureBox pb)
        {
            pb.Size = new Size(Form1.width, Form1.height);

            return pb;
        }
    }
}