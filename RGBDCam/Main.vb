
Public Class Main

    Dim oCam As New clRGBDCam
    Dim WithEvents oMap As New clMap
    Dim pen As New Pen(Color.DarkRed, 4)
    Dim brushgreen As New System.Drawing.Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalBrick, Color.LawnGreen, Me.BackColor)

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles Me.FormClosing
        oCam.Cancel()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Me.DoubleBuffered = True
        oCam.Start(oMap)
    End Sub

    Private Sub oMap_MapChanged() Handles oMap.MapChanged
        Try


            Using g = pictureBox1.CreateGraphics
                g.RotateTransform(180)
                g.TranslateTransform(-250, -500)
                g.Clear(Me.BackColor)
                g.FillPolygon(brushgreen, oMap.Path.ToArray)
                For Each Obstacle In oMap.ObstacleLines
                    If Obstacle.Count = 1 Then Obstacle.Add(Obstacle(0))
                    g.DrawLines(pen, Obstacle.ToArray)
                Next
                Me.InvokePaint(pictureBox1, New PaintEventArgs(g, pictureBox1.ClientRectangle))
            End Using
        Catch ex As Exception

        End Try

    End Sub
End Class

