Public Class Form1
    Dim oMap As New clMap

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        For i = 0 To 80
            oMap.SetPoint(New Point(r.Next(0, 500), r.Next(0, 500)), clMap.enState.Wall)
        Next
        Draw()
    End Sub

    Dim canvas As New Bitmap(1000, 1000)
    Private Sub Draw()
        Using g = Graphics.FromImage(canvas)
            g.Clear(pictureBox1.BackColor)
            oMap.Draw(g)
        End Using
        pictureBox1.Image = canvas
    End Sub

    Dim r As New Random(23)

    Private Sub Form1_Click(sender As Object, e As EventArgs) Handles Me.Click
        For i = 0 To 80
            'oMap.SetPoint(New Point(r.Next(0, 500), r.Next(0, 500)), clMap.enState.Wall)
        Next
        oMap.SetPoly({New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500)), New Point(r.Next(0, 500), r.Next(0, 500))}, clMap.enState.Green)
        Draw()
    End Sub
End Class
