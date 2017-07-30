using IBMiCmd.LanguageTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    class DragLabel : Label
    {
        public DragLabel()
        {
            MouseDown += labelWorker_MouseDown;
            MouseMove += labelWorker_MouseMove;
            MouseUp += labelWorker_MouseUp;
        }

        bool clicked = false;
        int iOldX;
        int iOldY;
        int iClickX;
        int iClickY;

        void labelWorker_MouseDown(object sender, MouseEventArgs e)
        {
            Label labelWorker = (Label)sender;
            if (e.Button == MouseButtons.Left)
            {
                Point p = ConvertFromChildToForm(e.X, e.Y, labelWorker);
                iOldX = p.X;
                iOldY = p.Y;
                iClickX = e.X;
                iClickY = e.Y;
                clicked = true;
            }
        }

        void labelWorker_MouseMove(object sender, MouseEventArgs e)
        {
            Label labelWorker = (Label)sender;
            if (clicked)
            {
                Point p = new Point(); // New Coordinate
                p.X = e.X + labelWorker.Left;
                p.Y = e.Y + labelWorker.Top;
                labelWorker.Left = p.X - iClickX;
                labelWorker.Top = p.Y - iClickY;
            }
        }

        void labelWorker_MouseUp(object sender, MouseEventArgs e)
        {
            Label labelWorker = (Label)sender;
            Point p = new Point(); // New Coordinate
            p.X = e.X + labelWorker.Left;
            p.Y = e.Y + labelWorker.Top;
            
            FieldInfo fieldInfo = (FieldInfo)labelWorker.Tag;
            fieldInfo.Position = PointToDSPF(PointToDSPFView(new Point(p.X - iClickX, p.Y - iClickY)), fieldInfo.Length);
            labelWorker.Location = dspfEdit.DSPFtoUILoc(fieldInfo.Position);

            clicked = false;
        }

        private Point ConvertFromChildToForm(int x, int y, Control control)
        {
            Point p = new Point(x, y);
            control.Location = p;
            return p;
        }

        private static Point PointToDSPFView(Point Location)
        {
            int x = (((int)Location.X) / 9) * 9;
            int y = (((int)Location.Y) / 19) * 19;

            return new Point(x, y);
        }

        private static Point PointToDSPF(Point Location, int fieldLen)
        {
            int x = (((int)Location.X) / 9)+1;
            int y = (((int)Location.Y) / 19)+1;

            if (x < 1) x = 1;
            if ((x + fieldLen) > dspfEdit.WindowSize.Width) x = dspfEdit.WindowSize.Width - fieldLen;
            if (y < 1) y = 1;
            if (y > dspfEdit.WindowSize.Height) y = dspfEdit.WindowSize.Height;

            return new Point(x, y);
        }
    }
}
