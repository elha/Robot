Public Class clMap
    Public Enum enState
        SubNodes = -100
        OffSite = -1
        Undefined = 0
        Green = 2
        Worked = 3
        Wall = 16
        Fence = 100
    End Enum
	Public lock As Object = Me
	Public Shared OffSitePen As Brush = Brushes.LightSalmon
    Public Shared FencePen As Brush = Brushes.Black
    Public Shared WallPen As Brush = Brushes.IndianRed
    Public Shared LawnPen As Brush = Brushes.LawnGreen
    Public Shared BorderPen As Pen = Pens.LightGray
	Public Shared WorkPen As Brush = Brushes.LightYellow

	Public Class clStats
        Public Count As Integer = 0
        Public Area As New Generic.Dictionary(Of enState, Integer)
        Public Sub New()
            Area(enState.Fence) = 0
            Area(enState.Green) = 0
            Area(enState.OffSite) = 0
            Area(enState.SubNodes) = 0
            Area(enState.Undefined) = 0
            Area(enState.Wall) = 0
            Area(enState.Worked) = 0
        End Sub
    End Class

    Public Structure Node
        Public Size As Integer
        Public Location As Point
        Public Value As enState

        Public SubNodes() As Node

        Public Sub Draw(g As Graphics, stat As clStats)
            stat.Count += 1
            Select Case Value
                Case enState.SubNodes
                    Array.ForEach(SubNodes, Sub(s) s.Draw(g, stat))
                Case enState.OffSite
                    g.FillRectangle(OffSitePen, Area)
                Case enState.Wall
                    g.FillRectangle(WallPen, Area)
                Case enState.Fence
                    g.FillRectangle(FencePen, Area)
                Case enState.Green
                    g.FillRectangle(LawnPen, Area)
                Case enState.Worked
                    g.FillRectangle(WorkPen, Area)
                Case Else
            End Select
            If SubNodes Is Nothing Then stat.Area(Value) += Size * Size
            'If Size > 1 Then g.DrawRectangle(BorderPen, Area)
        End Sub

        Public Function Area() As Rectangle
            Return New Rectangle(Location, New Size(Size, Size))
        End Function

        ''' <summary>
        ''' SetPoint @pLoc to Value
        ''' uses nRasterSize for Speed
        ''' e.g. green, Raster 16 = do not create subnodes if not really needed
        ''' but keep subnodes if existant
        ''' </summary>
        ''' <param name="pLoc"></param>
        ''' <param name="pValue"></param>
        ''' <param name="nTargetRasterSize"></param>
        Public Sub SetPoint(pLoc As Point, pValue As enState, nTargetRasterSize As Integer)
            'Node already has Value = done
            If Value = pValue Then SubNodes = Nothing : Return

            If Size <= nTargetRasterSize Then
                'Targetrastersize reached: SetValue
                If SubNodes IsNot Nothing Then
                    'If Subnodes: set Subnodes (undefined, subnodes)
                    For i = 0 To 3
                        SubNodes(i).SetPoint(pLoc, pValue, nTargetRasterSize)
                    Next
                ElseIf nTargetRasterSize = 1 Then
                    'always overwrite if TargetRasterSize = 1 
                    Value = pValue
                ElseIf Value < pValue Then
                    'keep Wall-Status, overwrite everything else
                    Value = pValue
                End If
            Else
                'need to write smaller Node
                Dim hSize = Size >> 1
                ' 0 1 
                ' 2 3
                If SubNodes Is Nothing Then
                    'create Nodes if needed
                    SubNodes = {
                                New Node With {.Value = Value, .Size = hSize, .Location = Location},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X + hSize, Location.Y)},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X, Location.Y + hSize)},
                                New Node With {.Value = Value, .Size = hSize, .Location = New Point(Location.X + hSize, Location.Y + hSize)}
                                }
                    Value = enState.SubNodes
                End If

                'SetPoint within Subnode
                SubNodes(If(pLoc.X > Location.X + hSize, 1, 0) + If(pLoc.Y > Location.Y + hSize, 2, 0)).SetPoint(pLoc, pValue, nTargetRasterSize)
            End If

            If SubNodes IsNot Nothing Then
                'clear Nodes if possible
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

        Friend Sub GetNodes(searchRect As Rectangle, searchValue() As enState, nodes As Generic.List(Of Node))
            If Not searchRect.IntersectsWith(Area) Then Return
            If SubNodes Is Nothing Then
                If searchValue.Contains(Value) Then nodes.Add(Me)
            Else
                For i = 0 To 3
                    SubNodes(i).GetNodes(searchRect, searchValue, nodes)
                Next
            End If
        End Sub

        Friend Function Corners() As IEnumerable(Of Point)
            If Size = 1 Then Return {Location}
            Return {Location,
                New Point(Location.X + Size - 1, Location.Y),
                New Point(Location.X, Location.Y + Size - 1),
                New Point(Location.X + Size - 1, Location.Y + Size - 1)
                            }
        End Function
    End Structure

    Friend Sub SetFence(p() As Point)
        Fence = p
        ExpandIfNecessary(Fence)
        MainNode.Value = enState.OffSite
        SetPoly(Fence, enState.Undefined, 16)
        SetPolyLine(Fence, enState.Fence, 16)
    End Sub

	Public Function Draw(g As Graphics) As clStats
		Dim out As New clStats
		MainNode.Draw(g, out)
		Return out
	End Function

	Private Sub ExpandIfNecessary(arrLoc() As Point)
		ExpandIfNecessary(BoundingBox(arrLoc))
	End Sub

	Private Sub ExpandIfNecessary(rectBound As Rectangle)
        While Not MainNode.Area.Contains(rectBound)
            Dim SubNode = MainNode
            Dim nSize = SubNode.Size
            Dim Location = SubNode.Location
            Dim Quadrant = 0
            If rectBound.X < SubNode.Location.X Then Location.X = SubNode.Location.X - nSize : Quadrant += 1
            If rectBound.Y < SubNode.Location.Y Then Location.Y = SubNode.Location.Y - nSize : Quadrant += 2

            MainNode = New Node() With {.Location = Location, .Size = nSize << 1}
            If SubNode.Value = enState.Undefined Then
                MainNode.Value = SubNode.Value
            Else
                MainNode.Value = enState.SubNodes
                MainNode.SubNodes = {
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = Location},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X + nSize, Location.Y)},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X, Location.Y + nSize)},
                New Node With {.Value = enState.Undefined, .Size = nSize, .Location = New Point(Location.X + nSize, Location.Y + nSize)}
                }
                MainNode.SubNodes(Quadrant) = SubNode
            End If
        End While
    End Sub

	Public Sub SetPoint(pViewPort As Point, nAngle As Double, pLoc As IEnumerable(Of Point), pValue As enState, Optional nRasterSize As Integer = 1)
		Dim arrLoc = Translate(pViewPort, nAngle, pLoc)
		SetPoint(arrLoc, pValue)
	End Sub

	Public Sub SetPolyLine(pLoc As Point(), pValue As enState, nRasterSize As Integer)
        SetPoint(PolygonToPoints(pLoc), pValue, nRasterSize)
    End Sub

	Public Sub SetPoly(pViewPort As Point, nAngle As Double, pLoc As Point(), pValue As enState, nRasterSize As Integer)
		SetPoly(mdTools.Translate(pViewPort, nAngle, pLoc), pValue, nRasterSize)
	End Sub

	Public Sub SetPoly(pLoc As Point(), pValue As enState, nRasterSize As Integer)
		Dim Box = BoundingBox(pLoc)
		ExpandIfNecessary(Box)
		Dim nXRasterFix = -Box.X Mod nRasterSize
        Dim nYRasterFix = -Box.Y Mod nRasterSize
        If nXRasterFix < 0 Then nXRasterFix += nRasterSize
        If nYRasterFix < 0 Then nYRasterFix += nRasterSize

        For x = Box.X + nXRasterFix To Box.Right Step nRasterSize
            For y = Box.Y + nYRasterFix To Box.Bottom Step nRasterSize
                Dim p = New Point(x, y)
                If IsInPolygon(pLoc, p) Then SetPoint({p}, pValue, nRasterSize)
            Next
        Next
    End Sub

	Private Sub SetPoint(arrLoc As IEnumerable(Of Point), pValue As enState, Optional nRasterSize As Integer = 1)
		For Each p In arrLoc
			MainNode.SetPoint(p, pValue, nRasterSize)
		Next
	End Sub

	Public Sub New()
		MainNode = New Node() With {.Value = enState.Undefined, .Location = New Point(0, 0), .Size = 2}
	End Sub

	Public MainNode As Node
	Public Fence As Point()

    Public Function FindNearestPoint(pSearch As Point, nValues() As enState, nRadius As Integer) As Point
        '1. Get all Nodes within aligned Area
        Dim SearchRect = New Rectangle(pSearch.X - nRadius, pSearch.Y - nRadius, 2 * nRadius, 2 * nRadius)
        Return FindNearestPoint(pSearch, Coners(SearchRect), nValues)
    End Function

    Public Function FindNearestPoint(pSearch As Point, pArea() As Point, nValues() As enState) As Point
        Dim Nodes As New Generic.List(Of Node)
        MainNode.GetNodes(BoundingBox(pArea), nValues, Nodes)

        '2. Find nearest Point
        Dim nDistance As Double = Double.MaxValue
        Dim out As New Point(Integer.MaxValue, Integer.MaxValue)
        For Each node In Nodes
            For Each Corner In node.Corners
                Dim nDist = (Corner.X - pSearch.X) ^ 2 + (Corner.Y - pSearch.Y) ^ 2
                If nDist < nDistance AndAlso IsInPolygon(pArea, Corner) Then out = Corner : nDistance = nDist
            Next
        Next
        Return out
    End Function
End Class
