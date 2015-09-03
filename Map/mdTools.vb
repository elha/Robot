Module mdTools

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

End Module
