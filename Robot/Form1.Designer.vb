<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Map = New System.Windows.Forms.Panel()
        Me.Sprite = New System.Windows.Forms.Panel()
        Me.Status = New System.Windows.Forms.TextBox()
        Me.Trace = New System.Windows.Forms.ListBox()
        Me.cmdL = New System.Windows.Forms.Button()
        Me.cmdF = New System.Windows.Forms.Button()
        Me.cmdR = New System.Windows.Forms.Button()
        Me.cmdB = New System.Windows.Forms.Button()
        Me.Map.SuspendLayout()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(88, 67)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(214, 72)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Map
        '
        Me.Map.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Map.BackColor = System.Drawing.SystemColors.ControlDark
        Me.Map.BackgroundImage = CType(resources.GetObject("Map.BackgroundImage"), System.Drawing.Image)
        Me.Map.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Map.Controls.Add(Me.Sprite)
        Me.Map.Location = New System.Drawing.Point(733, 23)
        Me.Map.Name = "Map"
        Me.Map.Size = New System.Drawing.Size(750, 670)
        Me.Map.TabIndex = 1
        '
        'Sprite
        '
        Me.Sprite.BackColor = System.Drawing.Color.Red
        Me.Sprite.Location = New System.Drawing.Point(129, 138)
        Me.Sprite.Name = "Sprite"
        Me.Sprite.Size = New System.Drawing.Size(10, 13)
        Me.Sprite.TabIndex = 0
        '
        'Status
        '
        Me.Status.Location = New System.Drawing.Point(12, 12)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(693, 26)
        Me.Status.TabIndex = 2
        '
        'Trace
        '
        Me.Trace.FormattingEnabled = True
        Me.Trace.ItemHeight = 20
        Me.Trace.Location = New System.Drawing.Point(12, 161)
        Me.Trace.Name = "Trace"
        Me.Trace.Size = New System.Drawing.Size(693, 444)
        Me.Trace.TabIndex = 3
        '
        'cmdL
        '
        Me.cmdL.Location = New System.Drawing.Point(357, 67)
        Me.cmdL.Name = "cmdL"
        Me.cmdL.Size = New System.Drawing.Size(59, 72)
        Me.cmdL.TabIndex = 4
        Me.cmdL.Text = "L"
        Me.cmdL.UseVisualStyleBackColor = True
        '
        'cmdF
        '
        Me.cmdF.Location = New System.Drawing.Point(422, 67)
        Me.cmdF.Name = "cmdF"
        Me.cmdF.Size = New System.Drawing.Size(59, 33)
        Me.cmdF.TabIndex = 5
        Me.cmdF.Text = "V"
        Me.cmdF.UseVisualStyleBackColor = True
        '
        'cmdR
        '
        Me.cmdR.Location = New System.Drawing.Point(487, 67)
        Me.cmdR.Name = "cmdR"
        Me.cmdR.Size = New System.Drawing.Size(59, 72)
        Me.cmdR.TabIndex = 6
        Me.cmdR.Text = "R"
        Me.cmdR.UseVisualStyleBackColor = True
        '
        'cmdB
        '
        Me.cmdB.Location = New System.Drawing.Point(422, 106)
        Me.cmdB.Name = "cmdB"
        Me.cmdB.Size = New System.Drawing.Size(59, 33)
        Me.cmdB.TabIndex = 7
        Me.cmdB.Text = "B"
        Me.cmdB.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1151, 654)
        Me.Controls.Add(Me.cmdB)
        Me.Controls.Add(Me.cmdR)
        Me.Controls.Add(Me.cmdF)
        Me.Controls.Add(Me.cmdL)
        Me.Controls.Add(Me.Trace)
        Me.Controls.Add(Me.Status)
        Me.Controls.Add(Me.Map)
        Me.Controls.Add(Me.Button1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Map.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Map As Panel
    Friend WithEvents Status As TextBox
    Friend WithEvents Trace As ListBox
    Friend WithEvents Sprite As Panel
    Friend WithEvents cmdL As Button
    Friend WithEvents cmdF As Button
    Friend WithEvents cmdR As Button
    Friend WithEvents cmdB As Button
End Class
