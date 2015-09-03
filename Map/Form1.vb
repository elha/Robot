Public Class Form1
    Dim oMap As New clMap

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        oMap.SetPoint(New Point(100, 100), clMap.enState.Wall)
        oMap.SetPoint(New Point(100, 200), clMap.enState.Wall)
        oMap.SetPoly({New Point(100, 100), New Point(200, 100), New Point(200, 200), New Point(100, 200)}, clMap.enState.Green)

        Using g = Panel1.CreateGraphics
            g.DrawRectangle(Pens.Aquamarine, New Rectangle(0, 0, 100, 100))
            oMap.Draw(g)
        End Using
        Panel1.Refresh()
    End Sub
End Class
