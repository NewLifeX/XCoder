using Gtk;

namespace XCoder.Tools
{
    partial class FrmSpeak
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
            this.components = new System.ComponentModel.Container();

            // 
            // richTextBox1
            // 
            this.richTextBox1 = new TextView();
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Buffer.Text = "学无先后达者为师！";
            // 
            // btnSpeak
            // 
            this.btnSpeak = new Button();
            this.btnSpeak.Name = "btnSpeak";
            this.btnSpeak.SetSizeRequest(51, 133);
            this.btnSpeak.Label = "文\n字\n转\n语\n音";
            this.btnSpeak.Clicked += new System.EventHandler(this.btnSpeak_Click);

            //
            // leftHBox
            //
            this.leftHBox = new HBox();
            this.leftHBox.SetSizeRequest(719, 426);
            this.leftHBox.PackStart(this.richTextBox1, true, true, 0);

            //
            // rightVBox
            //
            this.rightVBox = new VBox();
            this.rightVBox.PackStart(this.btnSpeak, false, false, 0);

            //
            // FrmSpeak
            //
            this.PackStart(this.leftHBox, true, true, 2);
            this.PackStart(this.rightVBox, false, false, 0);

            this.Name = "FrmSpeak";
            this.Halign = Align.Fill;
            this.Valign = Align.Fill;
            this.Orientation = Orientation.Horizontal;



        }

        #endregion
        private HBox leftHBox;
        private VBox rightVBox;

        private TextView richTextBox1;
        private Button btnSpeak;
    }
}