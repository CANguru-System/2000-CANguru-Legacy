using System;
using System.Drawing;
using System.Windows.Forms;

namespace DxfSegmenter
{
    public class TileSettings
    {
        public double Width { get; set; } = 410.0;   // Segmentlänge
        public double Height { get; set; } = 400.0;  // Segmenthöhe
        public double MaxSegmentLength { get; set; } = 400.0;

    }

    public class TileSettingsDialog : Form
    {
        private NumericUpDown nudWidth;
        private NumericUpDown nudHeight;
        private NumericUpDown nudMaxLen;
        private Button btnOk;
        private Button btnCancel;

        public double SegmentWidth => (double)nudWidth.Value;
        public double SegmentHeight => (double)nudHeight.Value;
        public double MaxSegmentLength => (double)nudMaxLen.Value;

        public TileSettingsDialog(TileSettings settings)
        {
            Text = "Segment-Einstellungen";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(300, 200);
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblWidth = new Label
            {
                Text = "Segmentbreite (mm):",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(lblWidth);

            nudWidth = new NumericUpDown
            {
                Location = new Point(160, 18),
                Width = 100,
                Minimum = 10,
                Maximum = 2000,
                DecimalPlaces = 1,
                Value = (decimal)settings.Width
            };
            Controls.Add(nudWidth);

            Label lblHeight = new Label
            {
                Text = "Segmenthöhe (mm):",
                Location = new Point(20, 60),
                AutoSize = true
            };
            Controls.Add(lblHeight);

            nudHeight = new NumericUpDown
            {
                Location = new Point(160, 58),
                Width = 100,
                Minimum = 10,
                Maximum = 2000,
                DecimalPlaces = 1,
                Value = (decimal)settings.Height
            };
            Controls.Add(nudHeight);
            Label lblMaxLen = new Label
            {
                Text = "Maximale Segmentlänge (mm):",
                Location = new Point(20, 100),
                AutoSize = true
            };
            Controls.Add(lblMaxLen);

            nudMaxLen = new NumericUpDown
            {
                Location = new Point(200, 98),
                Width = 80,
                Minimum = 10,
                Maximum = 2000,
                DecimalPlaces = 1,
                Value = (decimal)settings.MaxSegmentLength
            };
            Controls.Add(nudMaxLen);

            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(60, 150),
                DialogResult = DialogResult.OK
            };
            Controls.Add(btnOk);

            btnCancel = new Button
            {
                Text = "Abbrechen",
                Location = new Point(160, 150),
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
