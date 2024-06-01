#if TOOLS
using Godot;
using System.Collections.Generic;
using System.Reflection;

[Tool]
public partial class Marker2DPlugin : EditorPlugin
{
	EditorInspector inspector;
	private List<Vector2> points = new();
	private List<FieldInfo> fields = new();

	private bool IsVector2I;
	private int SelectedIndex;
	private bool pointSelected;

	private Node2D node;
	public override void _EnterTree()
	{
		inspector = EditorInterface.Singleton.GetInspector();
		// Initialization of the plugin goes here.
		inspector.PropertyEdited += OnPropertyEdited;
		// SetForceDrawOverForwardingEnabled();
		Input.UseAccumulatedInput = false;
	}

	private void OnPropertyEdited(string property)
	{
		_Edit(inspector.GetEditedObject());
	}
	public override void _ForwardCanvasDrawOverViewport(Control viewportControl)
	{
		for (int i = 0; i < points.Count; i++)
		{
			var point = ToViewportTransform(points[i], viewportControl, node);
			viewportControl.DrawCircle(point, 10, Colors.Black);
			if (pointSelected && SelectedIndex == i)
			{
				viewportControl.DrawCircle(point, 8, Colors.BlueViolet);
			}
			else
			{
				viewportControl.DrawCircle(point, 8, Colors.White);
			}
		}
	}
	private static Vector2 ToViewportTransform(Vector2 globalPoint, Control viewportControl, Node2D node)
	{
		return viewportControl.GetGlobalTransform().BasisXformInv(globalPoint) * node.GetViewportTransform().Scale + node.GetViewportTransform().Origin;
	}
	public override void _ExitTree()
	{
		inspector.PropertyEdited -= OnPropertyEdited;
	}

	public override void _Edit(GodotObject @object)
	{
		points.Clear();
		UpdateOverlays();
		if (@object == null)
		{
			return;
		}
		node = (Node2D)@object;
		inspector = EditorInterface.Singleton.GetInspector();
		FieldInfo[] info = @object.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (FieldInfo field in info)
		{
			if (!Marker2DAttribute.IsValidType(field.FieldType))
			{
				continue;
			}
			if (field.IsDefined(typeof(Marker2DAttribute)))
			{
				Vector2 point = (Vector2)field.GetValue(@object);
				points.Add(((Node2D)@object).ToGlobal(point));
				fields.Add(field);
			}
		}
	}
	public override bool _ForwardCanvasGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton e)
		{
			if (e.Pressed && e.ButtonIndex == MouseButton.Left)
			{
				for (int i = 0; i < points.Count; i++)
				{
					GD.Print(node.ToGlobal(node.GetLocalMousePosition()) + " " + points[i] + " " + e.GlobalPosition);
					if (node.ToGlobal(node.GetLocalMousePosition()).DistanceTo(points[i]) < 10)
					{
						SelectedIndex = i;
						pointSelected = true;
						UpdateOverlays();
						return true;
					}
				}
			}
			else if (e.ButtonIndex == MouseButton.Left)
			{
				pointSelected = false;
				UpdateOverlays();
				return true;
			}
		}
		if (@event is InputEventMouseMotion ev && pointSelected)
		{
			points[SelectedIndex] = node.ToGlobal(node.GetLocalMousePosition());
			fields[SelectedIndex].SetValue(node, node.ToLocal(points[SelectedIndex]));
			UpdateOverlays();
		}
		return false;
	}
	public override bool _Handles(GodotObject @object)
	{
		return @object is Node2D;
	}
}
#endif
