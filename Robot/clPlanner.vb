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
	Private Const cnPointRadius As Integer = 40
	Private Map As clMap
    Public Sub New(oMap As clMap)
        Map = oMap
    End Sub

	Public Class clWayPoint
		Public Point As Point
		Public Heading As Double
		Public Automatic As Boolean
	End Class

	Public Function CurrentWayPoint() As clWayPoint
		If WayPoints.Count = 0 Then Return Nothing
		Return WayPoints(0)
	End Function

	Public WayPoints As New Generic.List(Of clWayPoint)

	Public WallPoint As Point

    Dim ran As New Random(Now.Millisecond)

	Dim nMaxSpeed = 80 'cm/s

	Public Function RandomMow(pStart As Point, nHeading As Double) As Point
		Dim pWall = Map.FindNearestPoint(pStart, Translate(pStart, nHeading, {New Point(-30, 0), New Point(-50, 120), New Point(50, 120), New Point(30, 0)}), {clMap.enState.Fence, clMap.enState.Wall})
		If Distance(pStart, pWall) < 80 Then
			Dim nAngleWall = Heading(pStart, pWall)
			WallPoint = pWall
            'Head 60, WallNormal 90, Reflect 120+180
            '180 + 180 -60 = 300
            nHeading = Math.PI + 2 * nAngleWall - nHeading + (ran.NextDouble() - 0.5) * 0.2
		End If

		Dim pNext = Translate(New Point(0, 0), nHeading, {New Point(0, nMaxSpeed * 0.3)})(0)
		pNext.Offset(pStart)

		If Distance(pNext, Map.FindNearestPoint(pNext, {clMap.enState.Wall, clMap.enState.Fence}, 80)) < 40 Then Return RandomMow(pStart, nHeading)

		Return pNext
	End Function

	Public Function FollowFence(pStart As clWayPoint) As clWayPoint
		For Each i In {0, -5, -15, -25, -35, -45, -55}
			Dim nHeadingNext = pStart.Heading + i * 0.1
			Dim pPointNext = Translate(New Point(0, 0), nHeadingNext, {New Point(0, nMaxSpeed / 2)})(0)
			pPointNext.Offset(pStart.Point)
			Dim nDist = Distance(pPointNext, Map.FindNearestPoint(pPointNext, {clMap.enState.Wall, clMap.enState.Fence}, 200))
			If nDist > 60 Then Return New clWayPoint() With {.Point = pPointNext, .Heading = nHeadingNext, .Automatic = True}
		Next
		Return Nothing
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

	Friend Sub WayPointReached()
		WayPoints.RemoveAt(0)
	End Sub

	Public Sub RePlan()
		'find first problematic wps
		Dim i = 0
		While i < WayPoints.Count
			Dim wp = WayPoints(i)
			If Distance(wp.Point, Map.FindNearestPoint(wp.Point, {clMap.enState.Fence, clMap.enState.Wall}, cnPointRadius)) <= cnPointRadius Then
				WayPoints.RemoveRange(i, WayPoints.Count - i)
				'End If
				If i > 0 AndAlso wp.Automatic Then
				WayPoints.Remove(wp)
			Else
				i += 1
			End If
		End While

		'find next points
		If WayPoints.Count = 0 Then Return
		While WayPoints.Count < 5
			Dim oPoint = FollowFence(WayPoints(WayPoints.Count - 1))
			If oPoint Is Nothing Then Return
			WayPoints.Add(oPoint)
		End While

	End Sub

	Friend Sub ClearWaypoints()
		WayPoints.Clear()
	End Sub

	Friend Sub AddWayPoint(Point As Point, Heading As Double)
		WayPoints.Add(New clWayPoint() With {.Point = Point, .Heading = Heading})
	End Sub
End Class
