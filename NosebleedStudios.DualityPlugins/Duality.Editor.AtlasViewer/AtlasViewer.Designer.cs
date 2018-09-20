namespace Duality.Editor.AtlasViewer
{
    partial class AtlasViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.virtScroll = new System.Windows.Forms.VScrollBar();
            this.flowPanel = new Duality.Editor.AtlasViewer.DFPanel();
            this.SuspendLayout();
            // 
            // virtScroll
            // 
            this.virtScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.virtScroll.Location = new System.Drawing.Point(330, 0);
            this.virtScroll.Name = "virtScroll";
            this.virtScroll.Size = new System.Drawing.Size(17, 272);
            this.virtScroll.TabIndex = 1;
            // 
            // flowPanel
            // 
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel.Location = new System.Drawing.Point(0, 0);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(330, 272);
            this.flowPanel.TabIndex = 2;
            // 
            // AtlasViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 272);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.virtScroll);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "AtlasViewer";
            this.Text = "AtlasViewer";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.VScrollBar virtScroll;
        private DFPanel flowPanel;
    }
}