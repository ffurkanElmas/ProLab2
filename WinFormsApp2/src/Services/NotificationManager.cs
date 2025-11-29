using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2.src.Services
{
    public class NotificationManager
    {
        private readonly Form parentForm;

        public NotificationManager(Form form)
        {
            parentForm = form;
        }

        public void ShowToast(string text)
        {
            Label toast = new Label();
            toast.Text = text;
            toast.AutoSize = true;
            toast.BackColor = Color.FromArgb(40, 40, 40);
            toast.ForeColor = Color.White;
            toast.Padding = new Padding(10);
            toast.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            toast.BorderStyle = BorderStyle.FixedSingle;

            // Toast pozisyonu (sağ üst)
            toast.Location = new Point(
                parentForm.Width - toast.PreferredWidth - 30,
                20
            );

            toast.BringToFront();
            parentForm.Controls.Add(toast);

            // Fade-out timer
            System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 50;
            int opacity = 255;

            fadeTimer.Tick += (s, e) =>
            {
                opacity -= 15;

                if (opacity <= 0)
                {
                    fadeTimer.Stop();
                    parentForm.Controls.Remove(toast);
                    toast.Dispose();
                }
                else
                {
                    toast.ForeColor = Color.FromArgb(opacity, toast.ForeColor);
                    toast.BackColor = Color.FromArgb(opacity, toast.BackColor);
                }
            };

            // Toast görünme süresi
            System.Windows.Forms.Timer holdTimer = new System.Windows.Forms.Timer();
            holdTimer.Interval = 1500;
            holdTimer.Tick += (s, e) =>
            {
                holdTimer.Stop();
                fadeTimer.Start();
            };

            holdTimer.Start();
        }
    }
}
