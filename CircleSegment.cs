using System.Collections.Generic;
using Godot;

public partial class CircleSegment : Node2D
{
    // Minimum and maximum angles between segments
    private float _minDistance = Mathf.DegToRad(10); // Minimum distance in radians
    private float _maxDistance = Mathf.DegToRad(30); // Maximum distance in radians
    private float _radius = 100f; // Radius of the circle
    private int _segments = 16;   // Number of segments for a smooth curve

    private readonly List<Area2D> _collisionAreas = new();
    
    public override void _Ready()
    {
        // Ensure that _Draw() is called
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Clear all existing Area2D nodes related to fishing
        foreach (Area2D area in _collisionAreas)
        {
            if (area != null && area.IsInsideTree())
            {
                area.QueueFree();
            }
        }
        _collisionAreas.Clear();
        
        // Define the common parameters for the circle segments
        Vector2 center = new Vector2(0, 0); // Center of the circle
        Color fillColor = new Color(0, 1, 0, 1); // Green color

        // Generate a random number of segments (between 2 and 4)
        int numSegments = GD.RandRange(2, 5); // GD.RandRange(2, 5) generates 2, 3, or 4

        // Store the angles where each segment starts
        List<float> startingAngles = new List<float>();

        float currentAngle = (float)GD.RandRange(0, Mathf.Pi * 2);
        for (int i = 0; i < numSegments; i++)
        {
            // Add the current angle as the start of the segment
            startingAngles.Add(currentAngle);

            // Generate a random distance between the segments (minDistance to maxDistance)
            float distance = Mathf.Lerp(_minDistance, _maxDistance, GD.RandRange(0, 1));

            // Update the current angle for the next segment, ensuring it respects the distance
            currentAngle += Mathf.DegToRad(45) + distance; // 45 degrees for the base segment size plus random distance
        }
        
        for (int i = 0; i < numSegments; i++)
        {
            float startAngle = startingAngles[i];
            
            // Generate a random end angle between 15 and 45 degrees for each segment
            float endAngle = startAngle + Mathf.DegToRad(GD.RandRange(15, 45));

            // Create a list to store the points for the filled polygon
            Vector2[] points = new Vector2[_segments + 2]; // +2 for the center and the last arc point
            points[0] = center; // First point is the center of the circle

            // Calculate points along the arc for this segment
            for (int j = 0; j <= _segments; j++)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, j / (float)_segments);
                points[j + 1] = center + new Vector2(Mathf.Cos(angle) * _radius, Mathf.Sin(angle) * _radius);
            }

            // Create an array of colors for each vertex
            Color[] colors = new Color[points.Length];
            for (int k = 0; k < colors.Length; k++)
            {
                colors[k] = fillColor; // Assign the same color to all points
            }

            // Draw the filled polygon
            DrawPolygon(points, colors);

            // Now create an Area2D and add the CollisionPolygon2D as its child
            AddCollisionPolygonToArea(points);
        }
    }

    // Function to add a CollisionPolygon2D inside an Area2D for each segment
    private void AddCollisionPolygonToArea(Vector2[] points)
    {
        // Create the Area2D for the collision segment
        Area2D area2D = new Area2D();
        AddChild(area2D); // Add the Area2D to the scene

        // Create the CollisionPolygon2D
        CollisionPolygon2D collisionPolygon = new CollisionPolygon2D
        {
            Polygon = points // Assign the same points as the drawn polygon
        };

        // Add the CollisionPolygon2D as a child of the Area2D
        area2D.AddChild(collisionPolygon);
        
        // Optionally connect collision signals here (if needed for each segment)
        area2D.Connect("area_entered", new Callable(this, nameof(OnArea2DBodyEntered)));
        
        // Store reference to the created Area2D node for later removal
        _collisionAreas.Add(area2D);
    }

    // Signal handler for collision detection
    private void OnArea2DBodyEntered(Node body)
    {
        if (body.IsInGroup("Arrow"))
        {
            GD.Print("Arrow entered the circle segment!");
        }
    }
}
