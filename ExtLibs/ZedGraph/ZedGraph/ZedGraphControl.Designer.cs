namespace ZedGraph
{
	partial class ZedGraphControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.pointToolTip = new System.Windows.Forms.ToolTip( this.components );
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.SuspendLayout();
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Location = new System.Drawing.Point( 128, 0 );
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size( 17, 128 );
			this.vScrollBar1.TabIndex = 0;
			//this.vScrollBar1.MouseCaptureChanged += new System.EventHandler( this.ScrollBarMouseCaptureChanged );
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler( this.vScrollBar1_Scroll );
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Location = new System.Drawing.Point( 0, 128 );
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size( 128, 17 );
			this.hScrollBar1.TabIndex = 1;
			//this.hScrollBar1.MouseCaptureChanged += new System.EventHandler( this.ScrollBarMouseCaptureChanged );
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler( this.hScrollBar1_Scroll );
			// 
			// pointToolTip
			// 
			this.pointToolTip.AutoPopDelay = 5000;
			this.pointToolTip.InitialDelay = 100;
			this.pointToolTip.ReshowDelay = 0;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size( 61, 4 );
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler( this.contextMenuStrip1_Opening );
			// 
			// ZedGraphControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.contextMenuStrip1;
			this.Controls.Add( this.hScrollBar1 );
			this.Controls.Add( this.vScrollBar1 );
			this.Name = "ZedGraphControl";
			this.Resize += new System.EventHandler( this.ZedGraphControl_ReSize );
			this.KeyUp += new System.Windows.Forms.KeyEventHandler( this.ZedGraphControl_KeyUp );
			this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.ZedGraphControl_KeyDown );
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseWheel );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.ToolTip pointToolTip;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
	}
}
