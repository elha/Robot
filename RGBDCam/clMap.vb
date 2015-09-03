Imports Map

Public Class clMap
    Public Class clObstacle
        Implements IQuadObject
        Public x As Integer
        Public y As Integer

        Public ReadOnly Property Bounds As Rectangle Implements IQuadObject.Bounds
            Get
                Return New Rectangle(x, y, 1, 1)
            End Get
        End Property

        Public Event BoundsChanged As EventHandler Implements IQuadObject.BoundsChanged
    End Class

    Public QTree As New QuadTree(Of clObstacle)(New Size(1, 1), 16)
    Public Event MapChanged()

    Public Obstacle As New List(Of Point)
    Public Path As New List(Of Point)

    Friend Sub SetObstacle(point As Point)
        'QTree.Insert(New clObstacle With {.x = point.X, .y = point.Y})
        Obstacle.Add(point)
    End Sub

    Friend Sub SetPath(point As Point)
        Path.Add(point)
    End Sub

    Public Sub UpdateUI()
        RaiseEvent MapChanged()
    End Sub

    Friend Function ObstacleLines() As IEnumerable(Of List(Of Point))
        Dim out As New List(Of List(Of Point))
        If Obstacle.Count = 0 Then Return out

        Dim cur As New List(Of Point)
        Dim cp As Point = Obstacle(0)
        cur.Add(cp)
        For Each p In Obstacle

            If Math.Abs(cp.X - p.X) > 8 Then cur = New List(Of Point) : out.Add(cur)
            If Math.Abs(cp.Y - p.Y) > 8 Then cur = New List(Of Point) : out.Add(cur)
            cur.Add(p)
            cp = p
        Next

        Return From o In out Where o.Count > 0
    End Function

    Friend Sub Clear()
        Obstacle.Clear()
        Path.Clear()
        Path.Add(New Point(0, 0))
    End Sub
End Class
