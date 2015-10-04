Module mdTools
    Public Function PolygonToPoints(poly As Point()) As Point()
        Dim out As New Generic.List(Of Point)

        Dim oldPoint = poly(poly.Length - 1)

        For Each newPoint In poly
            Dim dX = newPoint.X - oldPoint.X
            Dim dY = newPoint.Y - oldPoint.Y
            Dim nMax = Math.Max(Math.Abs(dX), Math.Abs(dY))
            For n = 1 To nMax
                Dim nPartial = n / nMax
                out.Add(New Point(oldPoint.X + dX * nPartial, oldPoint.Y + dY * nPartial))
            Next
            oldPoint = newPoint
        Next
        Return out.ToArray
    End Function

    Public Function IsInPolygon(poly As Point(), p As Point) As Boolean
        Dim pLeft As Point, pRight As Point
        Dim inside As Boolean = False
        If poly.Length < 3 Then
            Return inside
        End If

        Dim oldPoint = New Point(poly(poly.Length - 1).X, poly(poly.Length - 1).Y)

        For Each newPoint In poly
            If (newPoint.X < p.X) = (p.X <= oldPoint.X) Then 'poly passes x-coordinate from left or right
                If newPoint.X > oldPoint.X Then
                    pLeft = oldPoint
                    pRight = newPoint
                Else
                    pLeft = newPoint
                    pRight = oldPoint
                End If

                If (p.Y - pLeft.Y) * (pRight.X - pLeft.X) < (p.X - pLeft.X) * (pRight.Y - pLeft.Y) Then inside = Not inside
            End If
            oldPoint = newPoint
        Next

        Return inside
    End Function

    Public Function BoundingBox(poly As Point()) As Rectangle
        Dim out As Rectangle
        If poly.Count = 0 Then Return out
        Dim pNE As Point = poly(0)
        Dim pSW As Point = poly(0)
        For Each p In poly
            If pNE.X > p.X Then pNE.X = p.X
            If pNE.Y > p.Y Then pNE.Y = p.Y
            If pSW.X < p.X Then pSW.X = p.X
            If pSW.Y < p.Y Then pSW.Y = p.Y
        Next

        Return New Rectangle(pNE.X, pNE.Y, pSW.X - pNE.X, pSW.Y - pNE.Y)
    End Function

	Friend Function Translate(pViewPort As Point, nAngle As Double, pLoc() As Point) As Point()
        '1. Turn
        '2. Translate
        Dim nCos As Double = Math.Sin(nAngle)
		Dim nSin As Double = Math.Cos(nAngle)
		Return (From p In pLoc Select New Point(p.X * nCos + p.Y * nSin + pViewPort.X, -p.X * nSin + p.Y * nCos + pViewPort.Y)).ToArray
	End Function

	Friend Function Distance(p1 As Point, p2 As Point) As Integer
        Try
            If p1.X = Integer.MaxValue OrElse p2.X = Integer.MaxValue Then Return Integer.MaxValue
            Return Math.Sqrt((p1.X - p2.X) ^ 2 + (p1.Y - p2.Y) ^ 2)
        Catch ex As Exception
            Return Integer.MaxValue
        End Try
    End Function

    Friend Function Heading(p1 As Point, p2 As Point) As Double
        Return Math.Atan2(p2.X - p1.X, p2.Y - p1.Y)
    End Function

    Friend Function Coners(rect As Rectangle) As Point()
        If rect.Width = 1 AndAlso rect.Height = 1 Then Return {rect.Location}
        Return {rect.Location,
                New Point(rect.X + rect.Width - 1, rect.Y),
                New Point(rect.X, rect.Y + rect.Height - 1),
                New Point(rect.X + rect.Width - 1, rect.Y + rect.Height - 1)
                            }

    End Function
End Module
