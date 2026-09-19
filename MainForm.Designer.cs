namespace DxfSegmenter
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private TrackView trackView1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem dateiToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem segmentToolStripMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            trackView1 = new DxfSegmenter.TrackView();
            menuStrip1 = new MenuStrip();
            dateiToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            segmentToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // trackView1
            // 
            trackView1.Location = new Point(0, 24);
            trackView1.Name = "trackView1";
            trackView1.Size = new Size(1000, 700);
            trackView1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { dateiToolStripMenuItem, segmentToolStripMenuItem, toolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1000, 24);
            menuStrip1.TabIndex = 1;
            // 
            // dateiToolStripMenuItem
            // 
            dateiToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem });
            dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            dateiToolStripMenuItem.Size = new Size(46, 20);
            dateiToolStripMenuItem.Text = "Datei";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(133, 22);
            openFileToolStripMenuItem.Text = "DXF öffnen";
            openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
            // 
            // segmentToolStripMenuItem
            // 
            segmentToolStripMenuItem.Name = "segmentToolStripMenuItem";
            segmentToolStripMenuItem.Size = new Size(117, 20);
            segmentToolStripMenuItem.Text = "Segment erzeugen";
            segmentToolStripMenuItem.Click += segmentToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(102, 20);
            toolStripMenuItem1.Text = "DXF Exportieren";
            toolStripMenuItem1.Click += exportSegmentsToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(1000, 724);
            Controls.Add(trackView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "DXF Segmenter";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private ToolStripMenuItem toolStripMenuItem1;
    }
}
