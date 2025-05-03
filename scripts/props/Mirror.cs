using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Mirror : CsgBox3D
{
    private bool IsVisibleOnCamera(Camera3D camera)
    {
        if (camera.GlobalPosition.Y - this.GlobalPosition.Y < -10.0) 
        {
            return false;
        }

        Vector3 halfSize = Size / 2.0f;

        var cornerPoints = new List<Vector3>
        {
            GlobalTransform * new Vector3(-halfSize.X, halfSize.Y, 0), // top left
            GlobalTransform * new Vector3(halfSize.X, halfSize.Y, 0),  // top right
            GlobalTransform * new Vector3(-halfSize.X, -halfSize.Y, 0), // bottom left
            GlobalTransform * new Vector3(halfSize.X, -halfSize.Y, 0)   // bottom right
        };

        int numPointsOutsideFrustum = 0;
        foreach (var point in cornerPoints)
        {
            if (!camera.IsPositionInFrustum(point))
                numPointsOutsideFrustum++;
        }

        if (numPointsOutsideFrustum == 4)
        {
            Plane[] frustumPlanes = camera.GetFrustum().ToArray();

            foreach (Plane plane in frustumPlanes)
            {
                bool allPointsOutside = plane.IsPointOver(cornerPoints[0])
                    && plane.IsPointOver(cornerPoints[1])
                    && plane.IsPointOver(cornerPoints[2])
                    && plane.IsPointOver(cornerPoints[3]);

                if (allPointsOutside)
                    return false;
            }
        }

        return true;
    }

    public override void _Process(double delta)
    {
        Camera3D curCamera = GetViewport().GetCamera3D();
        if (curCamera != null && IsVisibleOnCamera(curCamera) && (curCamera.GlobalPosition - GlobalPosition).Length() < 100.0f)
        {
            GetNode<SubViewport>("SubViewport").RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
        }
        else
        {
            GetNode<SubViewport>("SubViewport").RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled;
        }
    }
}
