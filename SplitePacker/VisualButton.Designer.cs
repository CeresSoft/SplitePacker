namespace SplitePacker
{
    partial class VisualButton
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.vbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vbutton
            // 
            this.vbutton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.vbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vbutton.FlatAppearance.BorderSize = 0;
            this.vbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.vbutton.Location = new System.Drawing.Point(0, 0);
            this.vbutton.Name = "vbutton";
            this.vbutton.Size = new System.Drawing.Size(137, 112);
            this.vbutton.TabIndex = 0;
            this.vbutton.UseVisualStyleBackColor = false;
            this.vbutton.Click += new System.EventHandler(this.vbutton_Click);
            this.vbutton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vbutton_MouseDown);
            this.vbutton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.vbutton_MouseUp);
            // 
            // VisualButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.vbutton);
            this.Name = "VisualButton";
            this.Size = new System.Drawing.Size(137, 112);
            this.Load += new System.EventHandler(this.VisualButton_Load);
            this.EnabledChanged += new System.EventHandler(this.VisualButton_EnabledChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button vbutton;
    }
}
