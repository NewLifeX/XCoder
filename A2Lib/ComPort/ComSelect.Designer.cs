namespace ALib.ComPort
{
    partial class ComSelect
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
            comFrom = new ComForm();
            SuspendLayout();
            // 
            // comFrom
            // 
            comFrom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comFrom.BaudRate = 115200;
            comFrom.DataBits = 8;
            comFrom.Location = new Point(12, 12);
            comFrom.Name = "comFrom";
            comFrom.Parity = System.IO.Ports.Parity.None;
            comFrom.Port = null;
            comFrom.SelectedPort = "COM13(VCP0)";
            comFrom.Size = new Size(357, 49);
            comFrom.StopBits = System.IO.Ports.StopBits.One;
            comFrom.TabIndex = 0;
            // 
            // ComSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 74);
            Controls.Add(comFrom);
            Name = "ComSelect";
            Text = "ComSelect";
            ResumeLayout(false);
        }

        #endregion

        public ComForm comFrom;
    }
}