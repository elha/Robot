Public Class clPlanner
    '1. Get Target
    '2. Plan Path
    '3. Avoid Collisions

    '########################
    '#                      #
    '#                      #
    '#     ##############   #
    '#     #            #   #
    '#     #            #   #
    '#  S  #    T       #   #
    '#     #                #
    '########################

    Private Map As clMap
    Public Sub New(oMap As clMap)
        Map = oMap
    End Sub

    Public Class clWayPoint
        Public Point As Point
        Public Heading As Double
    End Class

    Public WallPoint As Point

    Dim ran As New Random(Now.Millisecond)

	Dim nMaxSpeed = 80 'cm/s

    Public Function RandomMow(pStart As Point, nHeading As Double) As IEnumerable(Of clWayPoint)
        Dim out As New Generic.List(Of clWayPoint)
        Dim pWall = Map.FindNearestPoint(pStart, Translate(pStart, nHeading, {New Point(-30, 0), New Point(-50, 60), New Point(50, 60), New Point(30, 0)}), {clMap.enState.Fence, clMap.enState.Wall})
		If pWall.X < Integer.MaxValue Then
			Dim nAngleWall = Heading(pStart, pWall)
			WallPoint = pWall
            'Head 60, WallNormal 90, Reflect 120+180
            '180 + 180 -60 = 300
            nHeading = Math.PI + 2 * nAngleWall - nHeading + (ran.NextDouble() - 0.5) * 0.2
		End If

		Dim pNext = Translate(New Point(0, 0), nHeading, {New Point(0, nMaxSpeed)})(0)
		pNext.Offset(pStart)
		out.Add(New clWayPoint() With {.Point = pNext, .Heading = nHeading})

		Return out
	End Function

	Public Function FollowWall(pStart As Point, nHeading As Double) As Point
		Static Mode As Integer = 0
        'Mode
        '0 Find Wall
        '1 Follow Wall
        Dim pCollision = Map.FindNearestPoint(pStart, Translate(pStart, nHeading, {New Point(-30, 0), New Point(-50, 60), New Point(50, 60), New Point(30, 0)}), {clMap.enState.Fence, clMap.enState.Wall})
		Dim pLeftWall = Map.FindNearestPoint(pStart, Translate(pStart, nHeading, {New Point(-2000, 60), New Point(-2000, 100), New Point(0, 100), New Point(0, 60)}), {clMap.enState.Fence, clMap.enState.Wall})
		WallPoint = pLeftWall

		Select Case Mode
			Case 0 'drive to wall
                If pCollision.X = Integer.MaxValue OrElse Distance(pCollision, pStart) > 200 Then
                    'drive straight to wall
                    Return Translate(pStart, nHeading, {New Point(0, nMaxSpeed)})(0)
				Else
                    'Turn right set mode 1
                    nHeading -= Math.PI / 2
					Mode = 1
					Return Translate(pStart, nHeading, {New Point(0, 0)})(0)

				End If

			Case 1
				If pCollision.X < Integer.MaxValue AndAlso Distance(pCollision, pStart) < 60 Then
                    '        MsgBox("collision")
                End If

				Dim nDistance = 60
				Dim nWallDist = Distance(pStart, pLeftWall)
				If nWallDist = Integer.MaxValue Then Mode = 0 : Return pStart

				Dim nAngleWall = Heading(pStart, pLeftWall)
				Dim nDriveHeading = nAngleWall - Math.PI / 2
				If nWallDist < nDistance Then
					nDriveHeading -= 0.1
				Else
					nDriveHeading += 0.1
				End If

				Return Translate(pStart, nDriveHeading, {New Point(0, nMaxSpeed)})(0)

		End Select



	End Function

	Friend Function CalcPath(pRobot As Object, pDest As Object) As clPath
        Dim out As New clPath
        Return out
    End Function

    Public Class clPath
        Public Sub Draw(g As Graphics)

        End Sub
    End Class
End Class
