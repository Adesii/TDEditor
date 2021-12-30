using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace TDEditor.Editors
{
	public class TurretTreeView : ScrollArea
	{
		public TurretInstance CurrentTurretInstance;

		List<TurretComponentWidget> ComponentNodes = new();

		BoxLayout layout;
		public TurretTreeView( Widget parent = null ) : base( parent )
		{
			Canvas = new( Parent );
			layout = new( BoxLayout.Direction.TopToBottom, Canvas );
			layout.Spacing = 20;

			CurrentTurretInstance = TurretMainView.CurrentTurretInstance;

			SetSizeMode( SizeMode.CanGrow, SizeMode.CanGrow );
			VerticalScrollbarMode = ScrollbarMode.On;
			Log.Error( Size );

			//MinimumSize = new Vector2( 300, 200 );
			//MaximumSize = new Vector2( 200000, 200000 );
			//Size = new Vector2( 300, 0 );
			//SetBackgroundImage( "image/background-grid64.png" );
			TranslucentBackground = true;
			UpdatesEnabled = false;
			CreateUI();
			UpdatesEnabled = true;
		}

		Widget LastWidget;

		protected override void OnMouseMove( MouseEvent e )
		{
			base.OnMouseMove( e );
			if ( LastWidget == null || !LastWidget.IsUnderMouse )
			{
				LastWidget?.Update();
				//LastWidget = (GetItemAt( ToScene( e.LocalPosition ) ) as GraphicsWidget)?.Widget;

			}
			LastWidget?.Update();
		}
		[Event( "turret_editor_reload" )]
		public void NewInstance( TurretInstance instance )
		{
			Clear();
			if ( CurrentTurretInstance != null )
			{
				CurrentTurretInstance.Components.CollectionChanged -= HandleChanges;
			}
			CurrentTurretInstance = instance;
			CurrentTurretInstance.Components.CollectionChanged += HandleChanges;
		}
		public void HandleChanges( object? sender, NotifyCollectionChangedEventArgs e )
		{
			if ( e.Action == NotifyCollectionChangedAction.Add )
			{
				foreach ( KeyValuePair<Type, List<PropertyInfo>> item in e.NewItems )
				{
					AddChild( item.Key, item.Value );
				}
			}

		}
		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			if ( LastWidget != null )
			{
				LastWidget.Update();
				LastWidget = null;
			}
		}
		public void CreateUI()
		{
			Clear();

			ComponentNodes = new();
			if ( CurrentTurretInstance == null || CurrentTurretInstance.Components == null )
			{
				return;
			}
			foreach ( var item in CurrentTurretInstance?.Components )
			{
				AddChild( item.Key, item.Value );
			}
		}

		public void Clear()
		{
			foreach ( var item in ComponentNodes )
			{
				item?.Destroy();
			}
			Canvas?.Destroy();
			Canvas = new( Parent );
			layout = new( BoxLayout.Direction.TopToBottom, Canvas );
		}

		public void AddChild( Type type, List<PropertyInfo> properties )
		{

			var comp = new TurretComponentWidget( null, type, properties );
			comp.SetSizeMode( SizeMode.CanGrow, SizeMode.CanShrink );
			//comp.Title.Text = comp.Title.Text.PadLeft( (int)(Canvas.Size.x * 2.5f) );

			ComponentNodes.Add( comp );
			layout.Add( comp );
		}
	}

}
