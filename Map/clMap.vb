Public Class clMap
    Public Enum enState
        Undefined = 0
        SubNodes = -1
        Green = 1
        Wall = 16
    End Enum

    Public Shared WallPen As Brush = Brushes.IndianRed
    Public Shared LawnPen As Brush = Brushes.LawnGreen
    Public Shared BorderPen As Pen = Pens.LightGray

    Public Structure Node
        Public Size As Integer
        Public Location As Point
        Public Value As enState
        Public SubNodes() As Node

        Public Sub Draw(g As Graphics)
            Select Case Value
                Case enState.SubNodes
                    Array.ForEach(SubNodes, Sub(s) s.Draw(g))
                Case enState.Wall
                    g.FillRectangle(WallPen, Area)
                Case enState.Green
                    g.FillRectangle(LawnPen, Area)
                Case Else
            End Select
            If Size > 1 Then g.DrawRectangle(BorderPen, Area)
        End Sub

        Public Function Area() As Rectangle
            Return New Rectangle(Location, New Size(Size, Size))
        End Function

        Public Sub SetPoint(pLoc As Point, pValue As enState, nRasterSize As Integer)
            If Value = pValue Then SubNodes = Nothing : Return
            If Size <= nRasterSize Then
                'SetValue, if subnodes than there as well
                If SubNodes IsNot Nothing Then
                    For Each s In SubNodes
                        If nRasterSize = 1 OrElse s.Value = enState.Undefined OrElse s.Value = enState.SubNodes Then s.SetPoint(pLoc, pValue, nRasterSize)
                    Next
                Else
                    Value = pValue
                End If
            Else
                Dim hSize = Size >> 1
                ' 0 1 
                ' 2 3
                'create Nodes if needed
                If SubNodes Is Nothing Then
                    SubNodes = {
                                New Node With {.Value = Value, .Size = hSize, .Location = Location},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X + hSize, Location.Y)},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X, Location.Y + hSize)},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X + hSize, Location.Y + hSize)}
                                }
                    Value = enState.SubNodes
                End If

                'SetPoint within Subnode
                SubNodes(If(pLoc.X > Location.X + hSize, 1, 0) + If(pLoc.Y > Location.Y + hSize, 2, 0)).SetPoint(pLoc, pValue, nRasterSize)
            End If

            'clear Nodes if possible
            If SubNodes IsNot Nothing Then
                Dim nValue = SubNodes(0).Value
                If nValue <> enState.SubNodes AndAlso
                        nValue = SubNodes(1).Value AndAlso
                        nValue = SubNodes(2).Value AndAlso
                        nValue = SubNodes(3).Value Then
                    Value = nValue
                    SubNodes = Nothing
                End If
            End If
        End Sub

    End Structure

    Public Sub Draw(g As Graphics)
        MainNode.Draw(g)
    End Sub

    Public Sub SetPoint(pLoc As Point, pValue As enState, Optional nRasterSize As Integer = 2)
        'scale out if needed
        While Not MainNode.Area.Contains(pLoc)
            Dim SubNode = MainNode
            Dim nSize = SubNode.Size
            Dim Location = SubNode.Location
            Dim Quadrant = 0
            If pLoc.X < SubNode.Location.X Then Location.X = SubNode.Location.X - nSize : Quadrant += 1
            If pLoc.Y < SubNode.Location.Y Then Location.Y = SubNode.Location.Y - nSize : Quadrant += 2

            MainNode = New Node() With {.Location = Location, .Value = enState.SubNodes, .Size = nSize << 1, .SubNodes = {
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = Location},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X + nSize, Location.Y)},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X, Location.Y + nSize)},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X + nSize, Location.Y + nSize)}
                }
              }
            MainNode.SubNodes(Quadrant) = SubNode
        End While
        MainNode.SetPoint(pLoc, pValue, nRasterSize)
    End Sub

    Const nRasterSize = 16
    Public Sub SetPoly(pLoc As Point(), pValue As enState)
        Dim Box = BoundingBox(pLoc)
        For x = Box.X + nRasterSize - (Box.X Mod nRasterSize) To Box.Right Step nRasterSize
            For y = Box.Y + nRasterSize - (Box.Y Mod nRasterSize) To Box.Bottom Step nRasterSize
                Dim p = New Point(x, y)
                If IsInPolygon(pLoc, p) Then SetPoint(p, pValue, nRasterSize)
            Next
        Next
    End Sub

    Public Sub New()
        MainNode = New Node() With {.Value = enState.Undefined, .Location = New Point(0, 0), .Size = 2}
    End Sub

    Private MainNode As Node
End Class
